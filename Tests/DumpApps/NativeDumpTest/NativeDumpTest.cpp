// NativeDumpTest.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

using namespace std;

#pragma auto_inline(off)

int main(int argc, char** argv);

enum MyEnum
{
	enumEntry0,
	enumEntry1,
	enumEntry2,
	enumEntry3,
	enumEntry4,
	enumEntry5,
	enumEntry6,
};

class MyTestClass
{
public:
	enum MyEnumInner
	{
		simple0,
		simple1,
		simple2,
		simple3,
		simple4,
	};

	wstring string1;
	list<wstring> strings;
	wstring stringArray[100];
	vector<string> ansiStrings;
	map<wstring, string> stringMap;
	unordered_map<wstring, string> stringUMap;
	MyEnum enumeration;
	MyEnumInner innerEnumeration;

	static int staticVariable;
} globalVariable;

int MyTestClass::staticVariable = 1212121212;

int(*mainAddress)(int, char**) = main;

NO_INLINE void DefaultTestCase()
{
	MyTestClass * p = &globalVariable;
	MyTestClass ** q = &p;
	MyEnum e = enumEntry3;

	p->string1 = L"qwerty";
	p->strings.push_back(L"Foo");
	p->strings.push_back(L"Bar");
	p->ansiStrings.push_back("AnsiFoo");
	p->ansiStrings.push_back("AnsiBar");
	p->stringMap.insert(make_pair(L"foo", "ansiFoo"));
	p->stringMap.insert(make_pair(L"bar", "ansiBar"));
	p->stringUMap.insert(make_pair(L"foo", "ansiFoo"));
	p->stringUMap.insert(make_pair(L"bar", "ansiBar"));

	int testArray[10000];

	for (int i = 0; i < sizeof(testArray) / sizeof(testArray[0]); i++)
		testArray[i] = 0x12121212;

	// Test shared/weak pointers
	shared_ptr<int> sptr1 = make_shared<int>(5);
	weak_ptr<int> wptr1 = sptr1;
	shared_ptr<int> esptr1 = make_shared<int>(42);
	weak_ptr<int> ewptr1 = esptr1;
	shared_ptr<int> esptr2(new int);
	weak_ptr<int> ewptr2 = esptr2;

	esptr1 = nullptr;
	esptr2 = nullptr;

	// Generate the dump
	//throw std::bad_exception();
	int* a = nullptr;
	*a = 4;
}

NO_INLINE void InfiniteRecursionTestCase(int arg)
{
#ifdef _WIN32
	Sleep(100);
#else
	this_thread::sleep_for(chrono::milliseconds(100));
#endif
	InfiniteRecursionTestCase(arg + 1);
}

NO_INLINE int main(int argc, char** argv)
{

	int testCaseToRun = 0;

	if (argc == 2)
	{
		testCaseToRun = atoi(argv[1]);
	}

	switch (testCaseToRun)
	{
	case 0 : 
		DefaultTestCase();
		break;
	case 1:
		InfiniteRecursionTestCase(0);
		break;
	default:
		DefaultTestCase();
	}

	return 0;
}

struct DoubleTest
{
	double d;
	float f;
	int i;
} doubleTest{ 3.5, 2.5, 5 };
