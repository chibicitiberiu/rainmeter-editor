#include "StdAfx.h"
#include <cstdint>
#include "Meter.h"
#include "HandleManager.h"
#include "Exports_Common.h"

EXPORT int Meter_Destroy(int handle)
{
	Meter* meter = (Meter*) handle_get_resource(handle);

	if (meter != nullptr)
	{
		handle_free(handle);
		delete meter;
		return Results::Ok;
	}

	return Results::InvalidHandle;
}