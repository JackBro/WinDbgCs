// CsDebugScriptTest.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "CsDebugScript.h"

int main()
{
	ULONG version, flags;

	DebugExtensionInitialize(&version, &flags);
	execute(nullptr, "..\\samples\\script.cs");
	DebugExtensionUninitialize();
    return 0;
}
