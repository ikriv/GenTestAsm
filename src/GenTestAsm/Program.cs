using System;
using System.Collections;
using System.Text;
using System.IO;
using System.Reflection;

namespace GenTestAsm
{
    class Program
    {
        const string _usage =
@"GenTestAsm generates nUnit-runnable .NET assembly from Win32 DLL.
Usage:
    GenTestAsm [/nocompile] [/prefix:Foo] input.dll output.dll

    input.dll should be in current directory - no backslashes are allowed    
    /nocompile generates a .cs file instead of binary output
    /prefix    determines prefix for test exports in input.dll; default is UnitTest";

        private string _input;
        private string _output;
        private bool   _noCompile;
        private string _prefix = "UnitTest";

        private static void Usage()
        {
            Console.WriteLine(_usage);
        }

        static void Main(string[] args)
        {
            try
            {
                new Program().Run( args );
            }
            catch (Exception x)
            {
                Console.Error.WriteLine( "Error: {0}", x.Message );
            }
        }

        private bool ReadOptions(string[] args)
        {
            if (args.Length == 0)
            {
                Usage();
                return false;
            }

            int i=0;
            while (ProcessSingleOption(args[i])) ++i;

            // there must be at least 2 more arguments left
            if (i+2 > args.Length)
            {
                Usage();
                return false;
            }

            _input = args[i]; ++i;
            _output = args[i];
            return true;
        }

        private bool ProcessSingleOption(string option)
        {
            if (option == null) return false;
            if (!option.StartsWith("/")) return false;
            if (option == "/nocompile")
            {
                _noCompile = true;
                return true;
            }

            if (option.StartsWith( "/prefix:" ))
            {
                _prefix = option.Substring( "/prefix:".Length );
                return true;
            }

            return false; // not an option or unknown option
        }

        IList GetTrimmedExports(ICollection exports)
        {
            ArrayList result = new ArrayList();
            foreach (string s in exports)
            {
                if (s.StartsWith( _prefix ))
                {
                    result.Add( s.Substring(_prefix.Length) );
                }
            }

            return result;
        }

        private void Run(string[] args)
        {
            if (!ReadOptions(args)) return;
            IList exports = GetTrimmedExports(new PeFile(_input).GetExportedNames());
            if (exports.Count == 0)
            {
                Console.WriteLine( "There are no test exports. No output was generated" );
                return;
            }

            GenerateAssembly( exports );
        }

        private void GenerateCsFile(ICollection exports)
        {
            string fileName = _output + ".cs";
            using (TestSourceFile file = new TestSourceFile( fileName, _input, _prefix, exports ))
            {
                file.Generate();
            }

            Console.WriteLine("Wrote {0}", fileName);
        }

        private void GenerateAssembly(ICollection exports)
        {
            GenTestDll.CreateAssembly(_output, _input, _prefix, exports, _noCompile);
            if (_noCompile)
            {
                Console.WriteLine( "Wrote {0}.cs", _output );
            }
            else
            {
                Console.WriteLine( "Wrote {0}", _output );
                WriteThunkDll();
            }
        }
        
        private void WriteThunkDll()
        {
            Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream("GenTestAsm.GenTestAsmThunk.dll");
            
            string path = GetThunkDllPath();
            if (File.Exists(path)) return;
            
            using (FileStream f = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                byte[] buffer = new byte[s.Length];
                s.Read(buffer, 0, buffer.Length);
                f.Write(buffer, 0, buffer.Length);
                Console.WriteLine("Wrote {0}", path);
            }
        }
        
        private string GetThunkDllPath()
        {
            return Path.Combine( Path.GetDirectoryName(_output), "GenTestAsmThunk.dll" );
        }
    }
}
