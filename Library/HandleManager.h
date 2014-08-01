#pragma once

#include <cstdint>

int32_t handle_allocate (void* resource);
void* handle_get_resource (int32_t handle);
void handle_free (int32_t handle);