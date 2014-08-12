/*
  Copyright (C) 2001 Kimmo Pekkola

  This program is free software; you can redistribute it and/or
  modify it under the terms of the GNU General Public License
  as published by the Free Software Foundation; either version 2
  of the License, or (at your option) any later version.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.

  You should have received a copy of the GNU General Public License
  along with this program; if not, write to the Free Software
  Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
  */

#include "StdAfx.h"
#include "../Common/PathUtil.h"
#include "Rainmeter.h"
#include "System.h"
#include "Error.h"
#include "MeasureNet.h"
#include "MeasureCPU.h"
#include "MeterString.h"
#include "../Version.h"

using namespace Gdiplus;

enum TIMER
{
	TIMER_NETSTATS = 1
};
enum INTERVAL
{
	INTERVAL_NETSTATS = 120000
};


/*
** Constructor
**
*/
Rainmeter::Rainmeter() :
m_UseD2D(true),
m_Debug(false),
m_DesktopWorkAreaChanged(false),
m_DesktopWorkAreaType(false),
m_NormalStayDesktop(true),
m_DisableDragging(false),
m_CurrentParser(),
m_Window(),
m_Mutex(),
m_Instance(),
m_GDIplusToken(),
m_GlobalOptions()
{
	CoInitializeEx(nullptr, COINIT_APARTMENTTHREADED | COINIT_DISABLE_OLE1DDE);

	InitCommonControls();

	// Initialize GDI+.
	GdiplusStartupInput gdiplusStartupInput;
	GdiplusStartup(&m_GDIplusToken, &gdiplusStartupInput, nullptr);
}

/*
** Destructor
**
*/
Rainmeter::~Rainmeter()
{
	CoUninitialize();

	GdiplusShutdown(m_GDIplusToken);
}

Rainmeter& Rainmeter::GetInstance()
{
	static Rainmeter s_Rainmeter;
	return s_Rainmeter;
}

/*
** The main initialization function for the module.
**
*/
int Rainmeter::Initialize()
{
	m_Instance = GetModuleHandle(L"Rainmeter");
	m_UseCurrentDirectory = true;

	// Create a window
	WNDCLASS wc = {0};
	wc.lpfnWndProc = (WNDPROC) MainWndProc;
	wc.hInstance = m_Instance;
	wc.lpszClassName = RAINMETER_CLASS_NAME;
	ATOM className = RegisterClass(&wc);

	m_Window = CreateWindowEx(
		WS_EX_TOOLWINDOW,
		MAKEINTATOM(className),
		RAINMETER_WINDOW_NAME,
		WS_POPUP | WS_DISABLED,
		CW_USEDEFAULT,
		CW_USEDEFAULT,
		CW_USEDEFAULT,
		CW_USEDEFAULT,
		nullptr,
		nullptr,
		m_Instance,
		nullptr);

	if (!m_Window) return 1;

	// Set up logger
	Logger& logger = GetLogger();

	// TODO: figure if we need language library	

	// Create user skins, layouts, addons, and plugins folders if needed
	LogNoticeF(L"Path: %s", GetWorkDirectory().c_str());

	// Initialize static stuff
	System::Initialize(m_Instance);

	MeasureNet::InitializeStatic();
	MeasureCPU::InitializeStatic();
	MeterString::InitializeStatic();

	ResetStats();
	ReadStats();

	// Change the work area if necessary
	if (m_DesktopWorkAreaChanged)
	{
		UpdateDesktopWorkArea(false);
	}

	return 0;	// All is OK
}

void Rainmeter::Finalize()
{
	KillTimer(m_Window, TIMER_NETSTATS);

	DeleteAllUnmanagedMeterWindows();
	DeleteAllMeterWindows();
	DeleteAllUnmanagedMeterWindows();  // Redelete unmanaged windows caused by OnCloseAction

	System::Finalize();

	MeasureNet::UpdateIFTable();
	MeasureNet::UpdateStats();
	WriteStats(true);

	MeasureNet::FinalizeStatic();
	MeasureCPU::FinalizeStatic();
	MeterString::FinalizeStatic();

	// Change the work area back
	if (m_DesktopWorkAreaChanged)
	{
		UpdateDesktopWorkArea(true);
	}

	if (m_Mutex) ReleaseMutex(m_Mutex);
}

int Rainmeter::MessagePump()
{
	MSG msg;
	BOOL ret;

	// Run the standard window message loop
	while ((ret = GetMessage(&msg, nullptr, 0, 0)) != 0)
	{
		if (ret == -1)
		{
			break;
		}

		TranslateMessage(&msg);
		DispatchMessage(&msg);
	}

	return (int) msg.wParam;
}

LRESULT CALLBACK Rainmeter::MainWndProc(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam)
{
	switch (uMsg)
	{
		case WM_DESTROY:
			PostQuitMessage(0);
			break;

		case WM_COPYDATA:
		{
			COPYDATASTRUCT* cds = (COPYDATASTRUCT*) lParam;
			if (cds)
			{
				const WCHAR* data = (const WCHAR*) cds->lpData;
				if (cds->dwData == 1 && (cds->cbData > 0))
				{
					GetRainmeter().DelayedExecuteCommand(data);
				}
			}
		}
			break;

		case WM_TIMER:
			if (wParam == TIMER_NETSTATS)
			{
				MeasureNet::UpdateIFTable();
				MeasureNet::UpdateStats();
				GetRainmeter().WriteStats(false);
			}
			break;

		case WM_RAINMETER_DELAYED_REFRESH_ALL:
			GetRainmeter().RefreshAll();
			break;

		case WM_RAINMETER_DELAYED_EXECUTE:
			if (lParam)
			{
				// Execute bang
				WCHAR* bang = (WCHAR*) lParam;
				GetRainmeter().ExecuteCommand(bang, nullptr);
				free(bang);  // _wcsdup()
			}
			break;

		case WM_RAINMETER_EXECUTE:
			if (GetRainmeter().HasMeterWindow((MeterWindow*) wParam))
			{
				GetRainmeter().ExecuteCommand((const WCHAR*) lParam, (MeterWindow*) wParam);
			}
			break;

		default:
			return DefWindowProc(hWnd, uMsg, wParam, lParam);
	}

	return 0;
}

void Rainmeter::SetNetworkStatisticsTimer()
{
	static bool set = SetTimer(m_Window, TIMER_NETSTATS, INTERVAL_NETSTATS, nullptr) != 0;
}

void Rainmeter::ActivateSkin(std::wstring file)
{
	file = NormalizePath(file);

	// Verify that the skin is not already active
	std::map<std::wstring, MeterWindow*>::const_iterator iter = m_MeterWindows.find(file);
	if (iter != m_MeterWindows.end())
	{
		if (wcscmp(((*iter).second)->GetFileName().c_str(), file.c_str()) == 0)
		{
			LogWarningF((*iter).second, L"!ActivateConfig: \"%s\" already active", file.c_str());
			return;
		}
		else
		{
			// Deactivate the existing skin
			DeactivateSkin((*iter).second);
		}
	}

	// Verify whether the ini-file exists
	if (_waccess(file.c_str(), 0) == -1)
	{
		throw std::exception("Skin file not found");
	}

	CreateMeterWindow(file);
}

void Rainmeter::DeactivateSkin(MeterWindow* meterWindow)
{
	if (meterWindow)
	{
		meterWindow->Deactivate();
	}
}

void Rainmeter::ToggleSkin(std::wstring file)
{
	file = GetAbsolutePath(file);
	std::wstring folder = PathUtil::GetFolderFromFilePath(file);

	MeterWindow* meterWindow = GetMeterWindow(file);

	if (meterWindow)
	{
		DeactivateSkin(meterWindow);
	}
	else
	{
		ActivateSkin(file);
	}
}

void Rainmeter::CreateMeterWindow(std::wstring file)
{
	file = NormalizePath(file);
	std::wstring folder = PathUtil::GetFolderFromFilePath(file);
	MeterWindow* mw = new MeterWindow(folder, file.substr(folder.size())); //TODO: temporary workaround

	// Note: May modify existing key
	m_MeterWindows[file] = mw;

	mw->Initialize();
}

void Rainmeter::DeleteAllMeterWindows()
{
	auto it = m_MeterWindows.cbegin();
	while (it != m_MeterWindows.cend())
	{
		MeterWindow* mw = (*it).second;
		m_MeterWindows.erase(it);  // Remove before deleting MeterWindow

		delete mw;

		// Get next valid iterator (Fix for iterator invalidation caused by OnCloseAction)
		it = m_MeterWindows.cbegin();
	}

	m_MeterWindows.clear();
}

void Rainmeter::DeleteAllUnmanagedMeterWindows()
{
	for (auto it = m_UnmanagedMeterWindows.cbegin(); it != m_UnmanagedMeterWindows.cend(); ++it)
	{
		delete (*it);
	}

	m_UnmanagedMeterWindows.clear();
}

/*
** Removes the skin from m_MeterWindows. The skin should delete itself.
**
*/
void Rainmeter::RemoveMeterWindow(MeterWindow* meterWindow)
{
	for (auto it = m_MeterWindows.cbegin(); it != m_MeterWindows.cend(); ++it)
	{
		if ((*it).second == meterWindow)
		{
			m_MeterWindows.erase(it);
			return;
		}
	}
}

/*
** Adds the skin to m_UnmanagedMeterWindows. The skin should remove itself by calling RemoveUnmanagedMeterWindow().
**
*/
void Rainmeter::AddUnmanagedMeterWindow(MeterWindow* meterWindow)
{
	for (auto it = m_UnmanagedMeterWindows.cbegin(); it != m_UnmanagedMeterWindows.cend(); ++it)
	{
		if ((*it) == meterWindow)  // already added
		{
			return;
		}
	}

	m_UnmanagedMeterWindows.push_back(meterWindow);
}

void Rainmeter::RemoveUnmanagedMeterWindow(MeterWindow* meterWindow)
{
	for (auto it = m_UnmanagedMeterWindows.cbegin(); it != m_UnmanagedMeterWindows.cend(); ++it)
	{
		if ((*it) == meterWindow)
		{
			m_UnmanagedMeterWindows.erase(it);
			break;
		}
	}
}

bool Rainmeter::HasMeterWindow(const MeterWindow* meterWindow) const
{
	for (auto it = m_MeterWindows.begin(); it != m_MeterWindows.end(); ++it)
	{
		if ((*it).second == meterWindow)
		{
			return true;
		}
	}

	return false;
}

MeterWindow* Rainmeter::GetMeterWindow(const std::wstring& filePath)
{
	std::wstring filePathLower;
	std::transform(filePath.begin(), filePath.end(), filePathLower.begin(), ::tolower);

	if (m_MeterWindows.count(filePathLower) > 0)
		return m_MeterWindows.at(filePathLower);

	return nullptr;
}

MeterWindow* Rainmeter::GetMeterWindow(HWND hwnd)
{
	std::map<std::wstring, MeterWindow*>::const_iterator iter = m_MeterWindows.begin();
	for (; iter != m_MeterWindows.end(); ++iter)
	{
		if ((*iter).second->GetWindow() == hwnd)
		{
			return (*iter).second;
		}
	}

	return nullptr;
}

void Rainmeter::GetMeterWindowsByLoadOrder(std::multimap<int, MeterWindow*>& windows, const std::wstring& group)
{
	std::map<std::wstring, MeterWindow*>::const_iterator iter = m_MeterWindows.begin();
	for (; iter != m_MeterWindows.end(); ++iter)
	{
		MeterWindow* mw = (*iter).second;
		if (mw && (group.empty() || mw->BelongsToGroup(group)))
		{
			windows.insert(std::pair<int, MeterWindow*>(GetLoadOrder((*iter).first), mw));
		}
	}
}

void Rainmeter::SetLoadOrder(const std::wstring& file, int order)
{
	for (auto iter = m_SkinOrders.begin(); iter != m_SkinOrders.end(); ++iter)
	{
		if ((*iter).second == file)  // already exists
		{
			if ((*iter).first != order)
			{
				m_SkinOrders.erase(iter);
				break;
			}
			else
			{
				return;
			}
		}
	}

	m_SkinOrders.insert(std::make_pair(order, file));
}

int Rainmeter::GetLoadOrder(const std::wstring& filePath)
{
	for (auto pair : m_SkinOrders)
	{
		if (pair.second == filePath)
		{
			return pair.first;
		}
	}
	
	// LoadOrder not specified
	return 0;
}

void Rainmeter::ExecuteBang(const WCHAR* bang, std::vector<std::wstring>& args, MeterWindow* meterWindow)
{
	m_CommandHandler.ExecuteBang(bang, args, meterWindow);
}

/*
** Runs the given command or bang
**
*/
void Rainmeter::ExecuteCommand(const WCHAR* command, MeterWindow* meterWindow, bool multi)
{
	m_CommandHandler.ExecuteCommand(command, meterWindow, multi);
}

/*
** Executes command when current processing is done.
**
*/
void Rainmeter::DelayedExecuteCommand(const WCHAR* command)
{
	WCHAR* bang = _wcsdup(command);
	PostMessage(m_Window, WM_RAINMETER_DELAYED_EXECUTE, (WPARAM)nullptr, (LPARAM) bang);
}

/*
** Refreshes all active meter windows.
** Note: This function calls MeterWindow::Refresh() directly for synchronization. Be careful about crash.
**
*/
void Rainmeter::RefreshAll()
{
	// Read skins and settings
	LoadLayout();

	// Change the work area if necessary
	if (m_DesktopWorkAreaChanged)
	{
		UpdateDesktopWorkArea(false);
	}

	// Make the sending order by using LoadOrder
	std::multimap<int, MeterWindow*> windows;
	GetMeterWindowsByLoadOrder(windows);

	// Prepare the helper window
	System::PrepareHelperWindow();

	// Refresh all
	std::multimap<int, MeterWindow*>::const_iterator iter = windows.begin();
	for (; iter != windows.end(); ++iter)
	{
		MeterWindow* mw = (*iter).second;
		if (mw)
		{
			mw->Refresh(false, true);
		}
	}
}

bool Rainmeter::LoadLayout(std::wstring filename)
{
	if (filename.empty())
	{
		filename = m_LayoutFile;
	}

	if (_waccess(filename.c_str(), 0) == -1)
	{
		return false;
	}

	// Unload current meters
	DeleteAllUnmanagedMeterWindows();
	DeleteAllMeterWindows();

	// Load layout file
	WCHAR buffer[MAX_PATH];

	// Clear old settings
	m_DesktopWorkAreas.clear();

	ConfigParser parser;
	parser.Initialize(filename, nullptr, nullptr);

	// Read desktop work area settings
	const std::wstring& area = parser.ReadString(L"Rainmeter", L"DesktopWorkArea", L"");
	if (!area.empty())
	{
		m_DesktopWorkAreas[0] = parser.ParseRECT(area.c_str());
		m_DesktopWorkAreaChanged = true;
	}

	const size_t monitorCount = System::GetMonitorCount();
	for (UINT i = 1; i <= monitorCount; ++i)
	{
		_snwprintf_s(buffer, _TRUNCATE, L"DesktopWorkArea@%i", (int)i);
		const std::wstring& area = parser.ReadString(L"Rainmeter", buffer, L"");
		if (!area.empty())
		{
			m_DesktopWorkAreas[i] = parser.ParseRECT(area.c_str());
			m_DesktopWorkAreaChanged = true;
		}
	}

	m_DesktopWorkAreaType = parser.ReadBool(L"Rainmeter", L"DesktopWorkAreaType", false);

	for (auto iter = parser.GetSections().cbegin(); iter != parser.GetSections().end(); ++iter)
	{
		const WCHAR* section = (*iter).c_str();

		if (wcscmp(section, L"Rainmeter") == 0 ||
			wcscmp(section, L"TrayMeasure") == 0)
		{
			continue;
		}
		
		// Make sure there is a ini file available
		int active = parser.ReadInt(section, L"Active", 0);
		if (active > 0)
		{
			// TODO: fix, get 'active'th file from *iter folder
			ActivateSkin(*iter);

			// TODO: activate using 'order'
			int order = parser.ReadInt(section, L"LoadOrder", 0);
			SetLoadOrder(*iter, order);
		}
	}

	return true;
}

/*
** Applies given DesktopWorkArea and DesktopWorkArea@n.
**
*/
void Rainmeter::UpdateDesktopWorkArea(bool reset)
{
	bool changed = false;

	if (reset)
	{
		if (!m_OldDesktopWorkAreas.empty())
		{
			int i = 1;
			for (auto iter = m_OldDesktopWorkAreas.cbegin(); iter != m_OldDesktopWorkAreas.cend(); ++iter, ++i)
			{
				RECT r = (*iter);

				BOOL result = SystemParametersInfo(SPI_SETWORKAREA, 0, &r, 0);

				if (m_Debug)
				{
					std::wstring format = L"Resetting WorkArea@%i: L=%i, T=%i, R=%i, B=%i (W=%i, H=%i)";
					if (!result)
					{
						format += L" => FAIL";
					}
					LogDebugF(format.c_str(), i, r.left, r.top, r.right, r.bottom, r.right - r.left, r.bottom - r.top);
				}
			}
			changed = true;
		}
	}
	else
	{
		const size_t numOfMonitors = System::GetMonitorCount();
		const MultiMonitorInfo& monitorsInfo = System::GetMultiMonitorInfo();
		const std::vector<MonitorInfo>& monitors = monitorsInfo.monitors;

		if (m_OldDesktopWorkAreas.empty())
		{
			// Store old work areas for changing them back
			for (size_t i = 0; i < numOfMonitors; ++i)
			{
				m_OldDesktopWorkAreas.push_back(monitors[i].work);
			}
		}

		if (m_Debug)
		{
			LogDebugF(L"DesktopWorkAreaType: %s", m_DesktopWorkAreaType ? L"Margin" : L"Default");
		}

		for (UINT i = 0; i <= numOfMonitors; ++i)
		{
			std::map<UINT, RECT>::const_iterator it = m_DesktopWorkAreas.find(i);
			if (it != m_DesktopWorkAreas.end())
			{
				RECT r = (*it).second;

				// Move rect to correct offset
				if (m_DesktopWorkAreaType)
				{
					RECT margin = r;
					r = (i == 0) ? monitors[monitorsInfo.primary - 1].screen : monitors[i - 1].screen;
					r.left += margin.left;
					r.top += margin.top;
					r.right -= margin.right;
					r.bottom -= margin.bottom;
				}
				else
				{
					if (i != 0)
					{
						const RECT screenRect = monitors[i - 1].screen;
						r.left += screenRect.left;
						r.top += screenRect.top;
						r.right += screenRect.left;
						r.bottom += screenRect.top;
					}
				}

				BOOL result = SystemParametersInfo(SPI_SETWORKAREA, 0, &r, 0);
				if (result)
				{
					changed = true;
				}

				if (m_Debug)
				{
					std::wstring format = L"Applying DesktopWorkArea";
					if (i != 0)
					{
						WCHAR buffer[64];
						size_t len = _snwprintf_s(buffer, _TRUNCATE, L"@%i", i);
						format.append(buffer, len);
					}
					format += L": L=%i, T=%i, R=%i, B=%i (W=%i, H=%i)";
					if (!result)
					{
						format += L" => FAIL";
					}
					LogDebugF(format.c_str(), r.left, r.top, r.right, r.bottom, r.right - r.left, r.bottom - r.top);
				}
			}
		}
	}

	if (changed && System::GetWindow())
	{
		// Update System::MultiMonitorInfo for for work area variables
		SendMessageTimeout(System::GetWindow(), WM_SETTINGCHANGE, SPI_SETWORKAREA, 0, SMTO_ABORTIFHUNG, 1000, nullptr);
	}
}

/*
** Reads the statistics from the ini-file
**
*/
void Rainmeter::ReadStats()
{
	const WCHAR* statsFile = m_StatsFile.c_str();

	// If m_StatsFile doesn't exist, create it
	if (_waccess(statsFile, 0) == -1)
	{
		WritePrivateProfileSection(L"Statistics", L"", statsFile);
	}

	// Only Net measure has stats at the moment
	MeasureNet::ReadStats(m_StatsFile, m_StatsDate);
}

/*
** Writes the statistics to the ini-file. If bForce is false the stats are written only once per an appropriate interval.
**
*/
void Rainmeter::WriteStats(bool bForce)
{
	static ULONGLONG lastWrite = 0;

	ULONGLONG ticks = System::GetTickCount64();

	if (bForce || (lastWrite + INTERVAL_NETSTATS < ticks))
	{
		lastWrite = ticks;

		// Only Net measure has stats at the moment
		const WCHAR* statsFile = m_StatsFile.c_str();
		MeasureNet::WriteStats(statsFile, m_StatsDate);

		WritePrivateProfileString(nullptr, nullptr, nullptr, statsFile);
	}
}

/*
** Clears the statistics
**
*/
void Rainmeter::ResetStats()
{
	// Set the stats-date string
	tm* newtime;
	time_t long_time;
	time(&long_time);
	newtime = localtime(&long_time);
	m_StatsDate = _wasctime(newtime);
	m_StatsDate.erase(m_StatsDate.size() - 1);

	// Only Net measure has stats at the moment
	MeasureNet::ResetStats();
}

void Rainmeter::SetDisableDragging(bool dragging)
{
	m_DisableDragging = dragging;
}

void Rainmeter::SetUseD2D(bool enabled)
{
	m_UseD2D = enabled;

	RefreshAll();
}

std::wstring Rainmeter::GetAbsolutePath(const std::wstring& path)
{
	if (!PathUtil::IsAbsolute(path))
	{
		WCHAR buffer[MAX_PATH];
		std::wstring workdir = GetWorkDirectory();

		// Work directory not absolute
		if (!PathUtil::IsAbsolute(workdir))
		{
			// Get current directory
			WCHAR currentdir[MAX_PATH];
			GetCurrentDirectory(MAX_PATH, currentdir);

			// Get absolute work directory
			PathCombine(buffer, currentdir, workdir.c_str());
			workdir.assign(buffer);
		}

		// Combine
		PathCombine(buffer, workdir.c_str(), path.c_str());

		return buffer;
	}

	return path;
}

std::wstring Rainmeter::NormalizePath(const std::wstring& path)
{
	// Convert to absolute
	std::wstring res = GetAbsolutePath(path);
	
	// To lower
	std::transform(res.begin(), res.end(), res.begin(), ::towlower);

	// Return
	return res;
}