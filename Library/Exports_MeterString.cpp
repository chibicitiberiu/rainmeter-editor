#include "StdAfx.h"
#include <cstdint>
#include "MeterString.h"
#include "HandleManager.h"
#include "Exports_Common.h"

EXPORT int MeterString_Init(int* handle_result, int meterCanvasHandle, LPCWSTR name)
{
	MeterWindow* w = (MeterWindow*) handle_get_resource(meterCanvasHandle);

	if (w != nullptr)
	{
		MeterString* result = new MeterString(w, name);
		*handle_result = handle_allocate(result);

		return Results::Ok;
	}

	return Results::InvalidHandle;
}

EXPORT int MeterString_Destroy(int handle)
{
	MeterString* ms = (MeterString*) handle_get_resource(handle);

	if (ms != nullptr)
	{
		handle_free(handle);
		delete ms;
		return Results::Ok;
	}

	return Results::InvalidHandle;
}

