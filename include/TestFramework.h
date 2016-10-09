#pragma once
#include <functional>
#include <sstream>
#include <windows.h>

#pragma warning(push)
#pragma warning(disable:4267) // converting X to Y, possible loss of data

class Assert
{
public:
	template <class T1, class T2>
	static void AreEqual(T1 const& expected, T2 const& actual, char const* file, int line)
	{
		if (expected == actual) return;
		std::ostringstream s;
		s << "Expected:\n'" << expected << "'\nactual:\n'" << actual << "'" 
		  << "\nat " << file << " (" << line << ")"; // this generates warning C4267 for size_t
		throw std::runtime_error(s.str());
	}

	static void AreEqual( char const* expected, char const* actual, char const* file, int line )
	{
		if (strcmp(expected, actual) == 0) return;
		std::ostringstream s;
		s << "Expected:\n'" << expected << "'\nactual:\n'" << actual <<"'"
		  << "\nat " << file << " (" << line << ")";;
		throw std::runtime_error(s.str());
	}

	template <class T>
	static void IsNull( T* pointer, char const* file, int line )
	{
		if (pointer == 0) return;
		Fail( "expected NULL pointer", file, line );
	}

	template <class T>
	static void IsNotNull( T* pointer, char const* file, int line )
	{
		if (pointer != 0) return;
		Fail( "expected non-NULL pointer", file, line );
	}
	
	static void IsTrue( int expression, char const* file, int line )
	{
	    if (!expression)
	    {
	        Fail("expression was not true", file, line);
	    }
	}

	static void IsFalse( int expression, char const* file, int line )
	{
	    if (expression)
	    {
	        Fail("expression was not false", file, line);
	    }
	}
	
	static void Fail( const char* message, char const* file, int line )
	{
		std::ostringstream s;
		s << message << "\nat " << file << " (" << line << ")";
		throw std::runtime_error(s.str());
	}
	
	
};

inline BSTR NUnitTestImplementation(void (*function)())
{
    try
    {
        function();
    }
    catch (std::exception& e)
    {
        std::wostringstream s;    
        s << e.what();
        return SysAllocString( s.str().c_str() );
    }
    catch (...)
    {
        return SysAllocString(L"Unknown exception occurred");
    }
    
    return NULL;
}

#pragma warning(pop)

#define ASSERT_EQUAL(a,b) Assert::AreEqual(a,b,__FILE__,__LINE__)
#define ASSERT_IS_NULL(p) Assert::IsNull(p,__FILE__,__LINE__)
#define ASSERT_IS_NOT_NULL(p) Assert::IsNotNull(p,__FILE__,__LINE__)
#define ASSERT_FAIL(msg) Assert::Fail(msg,__FILE__,__LINE__)
#define ASSERT_IS_TRUE(expression) Assert::IsTrue(expression,__FILE__,__LINE__)
#define ASSERT_IS_FALSE(expression) Assert::IsFalse(expression,__FILE__,__LINE__)

#define TEST(name)                                  \
static void UnitTest##name##Impl();                     \
                                                    \
extern "C" __declspec(dllexport)                    \
BSTR UnitTest##name()                                   \
{                                                   \
    return NUnitTestImplementation(UnitTest##name##Impl); \
}                                                   \
                                                    \
static void UnitTest##name##Impl()                        
                                                    