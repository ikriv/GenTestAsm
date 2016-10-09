#if DEBUG
using System;
using NUnit.Framework;
using System.IO;

namespace GenTestAsm.Tests
{
    [TestFixture]
    public class TestTestSourceFile
    {
        [Test]
        public void SimpleSourceFile()
        {
            string[] exports = {"Name1", "Name2"};
            StringWriter output = new StringWriter();
            TestSourceFile f = new TestSourceFile("test.cs", "Foo.dll", "Prefix", exports, output);
            f.Generate();

            string actualText = output.ToString();
            Assert.AreEqual(_simpleSourceFile_ExpectedText, actualText);
        }

        const string _simpleSourceFile_ExpectedText =
        "using System;\r\n" +
        "using System.Runtime.InteropServices;\r\n" +
        "using NUnit.Framework;\r\n" +
        "\r\n" +
        "namespace Generated.Test\r\n" +
        "{\r\n" +
        "    [TestFixture]\r\n" +
        "    public class CppTest\r\n" +
        "    {\r\n" +
        "        [DllImport(\"GenTestAsmThunk.dll\", CallingConvention=CallingConvention.Cdecl)]\r\n" +
        "        [return:MarshalAs(UnmanagedType.BStr)]\r\n" +
        "        private static extern string RunTest([MarshalAs(UnmanagedType.LPStr)]string dll, [MarshalAs(UnmanagedType.LPStr)]string method);\r\n" +
        "\r\n" +
        "        [Test]\r\n" +
        "        public void Name1()\r\n" +
        "        {\r\n" +
        "            string result = RunTest(\"Foo.dll\", \"PrefixName1\");\r\n" +
        "            if (result != null) throw new AssertionException(result);\r\n" +
        "        }\r\n" +
        "\r\n" +
        "        [Test]\r\n" +
        "        public void Name2()\r\n" +
        "        {\r\n" +
        "            string result = RunTest(\"Foo.dll\", \"PrefixName2\");\r\n" +
        "            if (result != null) throw new AssertionException(result);\r\n" +
        "        }\r\n" +
        "\r\n" +
        "\r\n" +
        "    }\r\n" +
        "}\r\n" +
        "\r\n";
    }

}
#endif