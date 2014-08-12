#pragma once

#define EXPORT extern "C" _declspec(dllexport)

namespace Results {

	enum CallResult 
	{
		Ok = 0,
		InvalidHandle = 1
	};

}
