#include <windows.h>

typedef BSTR (*TestFunc)();

extern "C"
__declspec(dllexport)
BSTR __cdecl RunTest( LPCSTR dll, LPCSTR name )
{
    HMODULE hLib = LoadLibrary(dll);
    if (hLib == NULL) return SysAllocString(L"Failed to load test DLL");
    
    TestFunc func = (TestFunc)GetProcAddress(hLib, name);
    
    if (func == NULL) return SysAllocString(L"Entry point not found");
    
    BSTR result = func();
    FreeLibrary(hLib);
    return result;
}