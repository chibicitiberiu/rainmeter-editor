#include "StdAfx.h"
#include <cstdint>
#include "Group.h"
#include "HandleManager.h"
#include "Exports_Common.h"

EXPORT int Group_BelongsToGroup(bool* result, int32_t handle, LPWSTR str)
{
	Group* group = (Group*) handle_get_resource(handle);

	if (group != nullptr)
	{
		*result = group->BelongsToGroup(str);
		return Results::Ok;
	}

	return Results::InvalidHandle;
}

EXPORT int Group_Destroy(int32_t handle)
{
	Group* group = (Group*) handle_get_resource(handle);

	if (group != nullptr)
	{
		handle_free(handle);
		delete group;
		return Results::Ok;
	}

	return Results::InvalidHandle;
}