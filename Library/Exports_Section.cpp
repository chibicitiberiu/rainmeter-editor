#include "HandleManager.h"
#include "Exports_Common.h"
#include "Section.h"

EXPORT bool Section_GetName (LPCWCHAR* result, int32_t handle)
{
	Section* section = (Section*) handle_get_resource (handle);

	if (section != nullptr)
	{
		*result = section->GetName();
		return true;
	}

	return false;
}

EXPORT bool Section_GetOriginalName (LPCWCHAR* result, int32_t handle)
{
	Section* section = (Section*) handle_get_resource (handle);

	if (section != nullptr)
	{
		*result = section->GetOriginalName().c_str();
		return true;
	}

	return false;
}

EXPORT bool Section_HasDynamicVariables (bool* result, int32_t handle)
{
	Section* section = (Section*) handle_get_resource (handle);

	if (section != nullptr)
	{
		*result = section->HasDynamicVariables();
		return true;
	}

	return false;
}

EXPORT bool Section_SetDynamicVariables (int32_t handle, bool value)
{
	Section* section = (Section*) handle_get_resource (handle);

	if (section != nullptr)
	{
		section->SetDynamicVariables(value);
		return true;
	}

	return false;
}

EXPORT bool Section_ResetUpdateCounter (int32_t handle)
{
	Section* section = (Section*) handle_get_resource (handle);

	if (section != nullptr)
	{
		section->ResetUpdateCounter();
		return true;
	}

	return false;
}

EXPORT bool Section_GetUpdateCounter (int* result, int32_t handle)
{
	Section* section = (Section*) handle_get_resource (handle);

	if (section != nullptr)
	{
		*result = section->GetUpdateCounter();
		return true;
	}

	return false;
}


EXPORT bool Section_GetUpdateDivider (int* result, int32_t handle)
{
	Section* section = (Section*) handle_get_resource (handle);

	if (section != nullptr)
	{
		*result = section->GetUpdateDivider();
		return true;
	}

	return false;
}

EXPORT bool Section_GetOnUpdateAction (LPCWCHAR* result, int32_t handle)
{
	Section* section = (Section*) handle_get_resource (handle);

	if (section != nullptr)
	{
		*result = section->GetOnUpdateAction().c_str();
		return true;
	}

	return false;
}

EXPORT bool Section_DoUpdateAction (int32_t handle)
{
	Section* section = (Section*) handle_get_resource (handle);
	if (section != nullptr)
	{
		section->DoUpdateAction ();
		return true;
	}
	return false;
}

EXPORT bool Section_Destroy (int32_t handle)
{
	Section* section = (Section*) handle_get_resource (handle);
	if (section != nullptr)
	{
		handle_free (handle);
		delete section;
		return true;
	}
	return false;
}