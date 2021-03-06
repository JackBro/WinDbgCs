// The following ifdef block is the standard way of creating macros which make exporting 
// from a DLL simpler. All files within this DLL are compiled with the CSDEBUGSCRIPT_EXPORTS
// symbol defined on the command line. This symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see 
// CSDEBUGSCRIPT_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.
#ifdef CSDEBUGSCRIPT_EXPORTS
#define CSDEBUGSCRIPT_API __declspec(dllexport)
#else
#define CSDEBUGSCRIPT_API __declspec(dllimport)
#endif

struct IDebugClient;

extern "C" {

CSDEBUGSCRIPT_API HRESULT DebugExtensionInitialize(
	_Out_ PULONG Version,
	_Out_ PULONG Flags);

CSDEBUGSCRIPT_API void DebugExtensionUninitialize();

CSDEBUGSCRIPT_API HRESULT uninitialize(
	_In_     IDebugClient* client,
	_In_opt_ PCSTR         Args);

CSDEBUGSCRIPT_API HRESULT execute(
	_In_     IDebugClient* client,
	_In_opt_ PCSTR         Args);

CSDEBUGSCRIPT_API HRESULT interactive(
	_In_     IDebugClient* client,
	_In_opt_ PCSTR         Args);

CSDEBUGSCRIPT_API HRESULT openui(
	_In_     IDebugClient* client,
	_In_opt_ PCSTR         Args);

CSDEBUGSCRIPT_API HRESULT interpret(
	_In_     IDebugClient* client,
	_In_opt_ PCSTR         Args);

};

template<class T>
class CAutoComPtr
{
private:
	CAutoComPtr(CAutoComPtr&);

public:
	CAutoComPtr()
		: pointer(nullptr)
	{
	}

	CAutoComPtr(T* pointer)
		: pointer(pointer)
	{
	}

	~CAutoComPtr()
	{
		if (pointer)
			pointer->Release();
	}

	CAutoComPtr& operator=(T* p)
	{
		if (pointer)
			pointer->Release();
		pointer = p;
		return *this;
	}

	T* operator->() const
	{
		return pointer;
	}

	operator T*() const
	{
		return pointer;
	}

	T** operator&()
	{
		return &pointer;
	}

	bool operator !=(T* p) const
	{
		return pointer != p;
	}

	T* PvReturn()
	{
		T* result = pointer;

		pointer = nullptr;
		return result;
	}

private:
	T* pointer;
};
