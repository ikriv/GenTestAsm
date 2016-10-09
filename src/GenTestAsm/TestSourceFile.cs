using System;
using System.Collections;
using System.IO;

namespace GenTestAsm
{
    /// <summary>
    /// Generates .cs file for calling test code
    /// </summary>
    class TestSourceFile : IDisposable
    {
        private string _cppDllName;
        private ICollection _testNames;
        private string _name;
        private TextWriter _stream;
        private string _prefix;

        private const string _preamble = 
@"using System;
using System.Runtime.InteropServices;
using NUnit.Framework;

namespace Generated.Test
{
    [TestFixture]
    public class CppTest
    {
        [DllImport(""GenTestAsmThunk.dll"", CallingConvention=CallingConvention.Cdecl)]
        [return:MarshalAs(UnmanagedType.BStr)]
        private static extern string RunTest([MarshalAs(UnmanagedType.LPStr)]string dll, [MarshalAs(UnmanagedType.LPStr)]string method);
";

        private const string _coda =
@"
    }
}
";

        public TestSourceFile(string name, string cppDllName, string prefix, ICollection testNames)
            :
            this (name, cppDllName, prefix, testNames, new StreamWriter(name) )
        {
        }

        public TestSourceFile(string name, string cppDllName, string prefix, ICollection testNames, TextWriter stream)
        {
            _cppDllName = cppDllName;
            _testNames = testNames;
            _name = name;
            _prefix = prefix;
            _stream = stream;
        }

        public string Name
        {
            get { return _name; }
        }

        public void Generate()
        {
            WritePreamble();
            WriteTests();
            WriteCoda();
        }

        private void WritePreamble()
        {
            _stream.WriteLine( _preamble );
        }

        private void WriteCoda()
        {
            _stream.WriteLine(_coda);
        }

        private void WriteTests()
        {
            foreach (string test in _testNames)
            {
                WriteSingleTest( test );
            }
        }

        private void WriteSingleTest(string test)
        {
            _stream.WriteLine( "        [Test]" );
            _stream.WriteLine( String.Format( "        public void {0}()", test ) );
            _stream.WriteLine( "        {" );
            _stream.WriteLine( String.Format( "            string result = RunTest(\"{0}\", \"{1}\");", _cppDllName, _prefix + test ) );
            _stream.WriteLine( "            if (result != null) throw new AssertionException(result);" );
            _stream.WriteLine( "        }" );
            _stream.WriteLine();
        }

        public void  Dispose()
        {
            if (_stream != null)
            {
                _stream.Close();
                _stream = null;
            }
        }
    }
}
