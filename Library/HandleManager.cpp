#include <map>
#include <cstdint>
#include "HandleManager.h"

std::map<int32_t, void*> handles;

int32_t handle_allocate (void* resource)
{
	static int32_t handle = 1;

	handles.insert (std::make_pair(handle, resource));
	return handle++;
}

void* handle_get_resource (int32_t handle)
{
	if (handles.count (handle) != 0)
		return handles.at (handle);

	return nullptr;
}

void handle_free (int32_t handle)
{
	handles.erase (handle);
}

void 