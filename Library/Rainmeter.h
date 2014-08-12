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

#ifndef __RAINMETER_H__
#define __RAINMETER_H__

#include <windows.h>
#include <map>
#include <vector>
#include <list>
#include <string>
#include "CommandHandler.h"
#include "Logger.h"
#include "MeterWindow.h"
#include "../Common/PathUtil.h"

#define MAX_LINE_LENGTH 4096

#define APPNAME L"Rainmeter"
#ifdef _WIN64
#define APPBITS L"64-bit"
#else
#define APPBITS L"32-bit"
#endif
#define WIDEN2(x) L ## x
#define WIDEN(x) WIDEN2(x)
#define APPDATE WIDEN(__DATE__)

#define RAINMETER_CLASS_NAME	L"DummyRainWClass"
#define RAINMETER_WINDOW_NAME	L"Rainmeter control window"

#define WM_RAINMETER_DELAYED_REFRESH_ALL WM_APP + 0
#define WM_RAINMETER_DELAYED_EXECUTE     WM_APP + 1
#define WM_RAINMETER_EXECUTE             WM_APP + 2

struct GlobalOptions
{
	double netInSpeed;
	double netOutSpeed;
};

class ConfigParser;

class Rainmeter
{
public:

	#pragma region Singleton

	/// <brief>
	/// Gets the instance
	/// </brief>
	static Rainmeter& GetInstance();

	#pragma endregion

	int Initialize();
	void Finalize();

	int MessagePump();

	void SetNetworkStatisticsTimer();

	ConfigParser* GetCurrentParser() { return m_CurrentParser; }
	void SetCurrentParser(ConfigParser* parser) { m_CurrentParser = parser; }

	bool HasMeterWindow(const MeterWindow* meterWindow) const;

	MeterWindow* GetMeterWindow(const std::wstring& folderPath);
	MeterWindow* GetMeterWindowByINI(const std::wstring& ini_searching);

	MeterWindow* GetMeterWindow(HWND hwnd);
	void GetMeterWindowsByLoadOrder(std::multimap<int, MeterWindow*>& windows, const std::wstring& group = std::wstring());
	std::map<std::wstring, MeterWindow*>& GetAllMeterWindows() { return m_MeterWindows; }
	
	void RemoveMeterWindow(MeterWindow* meterWindow);
	void AddUnmanagedMeterWindow(MeterWindow* meterWindow);
	void RemoveUnmanagedMeterWindow(MeterWindow* meterWindow);

	void ActivateSkin(std::wstring file);
	void DeactivateSkin(MeterWindow* meterWindow);
	void ToggleSkin(std::wstring file);
	void ToggleSkinWithID(UINT id);

	std::wstring GetWorkDirectory()
	{
		if (m_UseCurrentDirectory)
		{
			WCHAR buffer[MAX_PATH];
			GetCurrentDirectoryW(MAX_PATH, buffer);
			return buffer;
		}

		return m_WorkDirectory;
	}

	std::wstring GetSkinPath() { return GetWorkDirectory() + L"Skins\\"; }
	std::wstring GetLayoutPath() { return GetWorkDirectory() + L"Layouts\\"; }
	std::wstring GetPluginPath() { return GetWorkDirectory() + L"Plugins\\"; }
	std::wstring GetUserPluginPath() { return GetWorkDirectory() + L"Plugins\\"; }
	std::wstring GetAddonPath() { return GetWorkDirectory() + L"Addons\\"; }

	std::wstring GetDrive() { return PathUtil::GetVolume(GetWorkDirectory()); }

	const std::wstring& GetStatsDate() { return m_StatsDate; }

	HWND GetWindow() { return m_Window; }

	HINSTANCE GetModuleInstance() { return m_Instance; }

	bool GetUseD2D() const { return m_UseD2D; }
	void SetUseD2D(bool enabled);
	bool GetDebug() const { return m_Debug; }

	GlobalOptions& GetGlobalOptions() { return m_GlobalOptions; }

	void UpdateStats();
	void ReadStats();
	void WriteStats(bool bForce);
	void ResetStats();

	bool GetDisableDragging() { return m_DisableDragging; }
	void SetDisableDragging(bool dragging);

	bool IsNormalStayDesktop() { return m_NormalStayDesktop; }

	bool IsMenuActive() { /* TODO: implement c# callback */ LogErrorF(L"IsMenuActive callback not implemented."); return false; }
	void ShowContextMenu(POINT pos, MeterWindow* mw) { /* TODO: implement c# callback */ LogErrorF(L"ShowContextMenu callback not implemented."); }
	void ShowSkinCustomContextMenu(POINT pos, MeterWindow* mw) { /* TODO: implement c# callback */ LogErrorF(L"ShowSkinCustomContextMenu callback not implemented."); }

	void ExecuteBang(const WCHAR* bang, std::vector<std::wstring>& args, MeterWindow* meterWindow);
	void ExecuteCommand(const WCHAR* command, MeterWindow* meterWindow, bool multi = true);
	void DelayedExecuteCommand(const WCHAR* command);

	void RefreshAll();

	bool LoadLayout(std::wstring filename = std::wstring());

	friend class CommandHandler;

private:

	#pragma region Constructor, destructor

	Rainmeter();
	~Rainmeter();

	Rainmeter(const Rainmeter& other) = delete;
	Rainmeter& operator=(Rainmeter other) = delete;
	
	#pragma endregion

	static LRESULT CALLBACK MainWndProc(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam);

	void CreateMeterWindow(std::wstring file);
	void DeleteAllMeterWindows();
	void DeleteAllUnmanagedMeterWindows();
	void ReadGeneralSettings(const std::wstring& iniFile);
	void SetLoadOrder(const std::wstring& file, int order);
	int GetLoadOrder(const std::wstring& folderPath);
	void UpdateDesktopWorkArea(bool reset);
	std::wstring GetAbsolutePath(const std::wstring& path);
	std::wstring NormalizePath(const std::wstring& path);

	#pragma region Private fields

	std::multimap<int, std::wstring> m_SkinOrders;
	std::map<std::wstring, MeterWindow*> m_MeterWindows;
	std::list<MeterWindow*> m_UnmanagedMeterWindows;

	std::wstring m_WorkDirectory;
	bool m_UseCurrentDirectory;		// TODO: getter, setter
	std::wstring m_StatsFile;	// TODO: getter, setter
	std::wstring m_LayoutFile; // TODO: getter, setter

	std::wstring m_StatsDate;

	bool m_UseD2D;

	bool m_DesktopWorkAreaChanged;
	bool m_DesktopWorkAreaType;
	std::map<UINT, RECT> m_DesktopWorkAreas;
	std::vector<RECT> m_OldDesktopWorkAreas;

	bool m_NormalStayDesktop;
	bool m_Debug;

	bool m_DisableDragging;

	CommandHandler m_CommandHandler;

	ConfigParser* m_CurrentParser;

	HWND m_Window;

	HANDLE m_Mutex;
	HINSTANCE m_Instance;

	ULONG_PTR m_GDIplusToken;

	GlobalOptions m_GlobalOptions;
	
	#pragma endregion
};

// Convenience function.
inline Rainmeter& GetRainmeter() { return Rainmeter::GetInstance(); }

#endif
