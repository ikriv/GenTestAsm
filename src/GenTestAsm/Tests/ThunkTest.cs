#if DEBUG
using System;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Resources;
using System.IO;
using NUnit.Framework;

namespace GenTestAsm.Tests
{
    [TestFixture]
	public class ThunkTest
	{
        [DllImport(@"..\..\src\GenTestAsm\GenTestAsmThunk.dll", CallingConvention=CallingConvention.Cdecl)]
        [return:MarshalAs(UnmanagedType.BStr)]
        private static extern string RunTest([MarshalAs(UnmanagedType.LPStr)]string dll, [MarshalAs(UnmanagedType.LPStr)]string method);
	
	    [Test]
		public void CallThunk()
		{
		    string result = RunTest(@"..\..\samples\CppTestDll\Debug\CppTestDll.dll", "UnitTestOne");
		    Assert.IsNull(result);
		}

        [Test]
        public void CallThunk2()
        {
            string result = RunTest(@"..\..\samples\CppTestDll\Debug\CppTestDll.dll", "UnitTestTwo");
            Assert.IsTrue( result.StartsWith("Test failed!\nat "));
        }
        
        [Test]
        public void GetThunkResource()
        {
            Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream("GenTestAsm.GenTestAsmThunk.dll");
            Assert.IsNotNull(s);
        }
		
	}
}
#endif