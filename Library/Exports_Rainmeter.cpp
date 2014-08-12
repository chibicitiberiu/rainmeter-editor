#include "Rainmeter.h"
#include "HandleManager.h"

/*
** Initializes Rainmeter.
**
*/
bool Rainmeter_Initialize()
{
	int res = GetRainmeter().Initialize();

	// Success?
	if (res == 0)
		return &GetRainmeter();

	return nullptr;
}

/*
** Finalizes Rainmeter.
**
*/
void Rainmeter_Finalize(void* ptr)
{
	Rainmeter* rainmeter = (Rainmeter*) ptr;
	rainmeter->Finalize();
}
