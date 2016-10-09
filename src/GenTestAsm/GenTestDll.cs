using System;
using System.Collections;
using System.IO;
using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CSharp;

namespace GenTestAsm
{
    /// <summary>
    /// Generates a test DLL
    /// </summary>
    public class GenTestDll : IDisposable
    {
        private readonly string _outputDll;
        private readonly string _inputDll;
        private readonly ICollection _testNames;
        private string _testSourceFileName;
        private bool _noCompile;
        private string _prefix;
        private string[] _references;

        public static void CreateAssembly(string outputDll, string inputDll, string prefix, ICollection testNames, bool noCompile)
        {
            using (GenTestDll instance = new GenTestDll( outputDll, inputDll, prefix, testNames, noCompile ))
            {
                instance.CreateAssemblyImpl();
            }
        }

        private GenTestDll(string outputDll, string inputDll, string prefix, ICollection testNames, bool noCompile)
        {
            _outputDll = outputDll;
            _inputDll = inputDll;
            _testNames = testNames;
            _noCompile = noCompile;
            _prefix = prefix;
            _references = GetReferences();
        }
        
        private string[] GetReferences()
        {
            return new string[] { System.Configuration.ConfigurationSettings.AppSettings["nUnit.Reference"] };
        }

        private void CreateAssemblyImpl()
        {
            CreateTestSource();
            if (_noCompile) return;

            CodeDomProvider provider = new CSharpCodeProvider();

            CompilerResults compilerResults = 
                provider.CreateCompiler().CompileAssemblyFromFile( GetCompilerParameters(), _testSourceFileName );

            DisplayCompilerResults(compilerResults);
        }

        private static string GenerateTempFileName()
        {
            return Path.Combine( Path.GetTempPath(), Path.GetTempFileName() + ".cs" );
        }

        private void CreateTestSource()
        {
            string fileName = _noCompile ? (_outputDll + ".cs") : GenerateTempFileName();
            using (TestSourceFile testSourceFile = CreateTestSourceFileObj(fileName))
            {
                testSourceFile.Generate();
                _testSourceFileName = testSourceFile.Name;
            }
        }

        private TestSourceFile CreateTestSourceFileObj(string fileName)
        {
            return new TestSourceFile(fileName, Path.GetFileName(_inputDll), _prefix, _testNames);
        }
        
        private CompilerParameters GetCompilerParameters()
        {
            return new CompilerParameters( _references, _outputDll, true );
        }

        private void DisplayCompilerResults( CompilerResults results )
        {
            string msg = String.Format("Compiled to {0}. {1} errors", results.PathToAssembly, results.Errors.Count);
            Console.WriteLine(msg);

            foreach (string s in results.Output)
            {
                Console.WriteLine(s);
            }
        }

        public void Dispose()
        {
            if (!_noCompile && _testSourceFileName != null)
            {
                File.Delete( _testSourceFileName );
            }
        }
    }
}
