#include <windows.h>
#include "TestFramework.h"

TEST(One)
{
}

TEST(Two)
{
    ASSERT_FAIL("Test failed!");
}

TEST(Three)
{
    ASSERT_EQUAL(4, 2*2);
}

TEST(Four)
{
    ASSERT_EQUAL(5, 2*2);
}
