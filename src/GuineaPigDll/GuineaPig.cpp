#include <windows.h>
#include <stdio.h>

// this is a file with a handful of exports, to do our tests
extern "C"
__declspec(dllexport)
void WINAPI ExportA()
{
    printf("I am export A\n");
}

extern "C" 
__declspec(dllexport)
void __cdecl ExportB()
{
    printf("I am export B\n");
}

__declspec(dllexport)
void ExportC()
{
    printf("I am export C\n");
}

