#include "StdAfx.h"
#include <cstdint>
#include "Group.h"
#include "HandleManager.h"
#include "Exports_Common.h"

EXPORT bool Group_BelongsToGroup (bool* result, int32_t handle, LPWSTR str)
{
	Group* group = (Group*)handle_get_resource (handle);

	if (group != nullptr)
	{
		*result = group->BelongsToGroup (str);
		return true;
	}

	return false;
}

EXPORT bool Group_Destroy (int32_t handle)
{
	Group* group = (Group*)handle_get_resource (handle);

	if (group != nullptr)
	{
		handle_free (handle);
		delete group;
		return true;
	}

	return false;
}