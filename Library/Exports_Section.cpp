#include "HandleManager.h"
#include "Exports_Common.h"
#include "Section.h"

EXPORT int Section_GetName(LPCWCHAR* result, int32_t handle)
{
	Section* section = (Section*) handle_get_resource(handle);

	if (section != nullptr)
	{
		*result = section->GetName();
		return Results::Ok;
	}

	return Results::InvalidHandle;
}

EXPORT int Section_GetOriginalName(LPCWCHAR* result, int32_t handle)
{
	Section* section = (Section*) handle_get_resource(handle);

	if (section != nullptr)
	{
		*result = section->GetOriginalName().c_str();
		return Results::Ok;
	}

	return Results::InvalidHandle;
}

EXPORT int Section_HasDynamicVariables(bool* result, int32_t handle)
{
	Section* section = (Section*) handle_get_resource(handle);

	if (section != nullptr)
	{
		*result = section->HasDynamicVariables();
		return Results::Ok;
	}

	return Results::InvalidHandle;
}

EXPORT int Section_SetDynamicVariables(int32_t handle, bool value)
{
	Section* section = (Section*) handle_get_resource(handle);

	if (section != nullptr)
	{
		section->SetDynamicVariables(value);
		return Results::Ok;
	}

	return Results::InvalidHandle;
}

EXPORT int Section_ResetUpdateCounter(int32_t handle)
{
	Section* section = (Section*) handle_get_resource(handle);

	if (section != nullptr)
	{
		section->ResetUpdateCounter();
		return Results::Ok;
	}

	return Results::InvalidHandle;
}

EXPORT int Section_GetUpdateCounter(int* result, int32_t handle)
{
	Section* section = (Section*) handle_get_resource(handle);

	if (section != nullptr)
	{
		*result = section->GetUpdateCounter();
		return Results::Ok;
	}

	return Results::InvalidHandle;
}


EXPORT int Section_GetUpdateDivider(int* result, int32_t handle)
{
	Section* section = (Section*) handle_get_resource(handle);

	if (section != nullptr)
	{
		*result = section->GetUpdateDivider();
		return Results::Ok;
	}

	return Results::InvalidHandle;
}

EXPORT int Section_GetOnUpdateAction(LPCWCHAR* result, int32_t handle)
{
	Section* section = (Section*) handle_get_resource(handle);

	if (section != nullptr)
	{
		*result = section->GetOnUpdateAction().c_str();
		return Results::Ok;
	}

	return Results::InvalidHandle;
}

EXPORT int Section_DoUpdateAction(int32_t handle)
{
	Section* section = (Section*) handle_get_resource(handle);
	if (section != nullptr)
	{
		section->DoUpdateAction();
		return Results::Ok;
	}
	return Results::InvalidHandle;
}

EXPORT int Section_Destroy(int32_t handle)
{
	Section* section = (Section*) handle_get_resource(handle);
	if (section != nullptr)
	{
		handle_free(handle);
		delete section;
		return Results::Ok;
	}
	return Results::InvalidHandle;
}