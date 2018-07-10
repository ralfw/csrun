using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using csrun.data.domain;
using csrun.domain.compiletime;
using csrun.domain.runtime;
using Microsoft.CSharp;
using NUnit.Framework;
using CompilerError = System.CodeDom.Compiler.CompilerError;

namespace csrun.tests
{
    [TestFixture]
    public class CSCompiler_tests
    {
        [SetUp]
        public void Setup() {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
        }


        [Test]
        public void Compile_no_errors()
        {
            var csSource = new Sourcecode
            {
                Filename = "program.cs",
                Text = @"using System;
public class Program {
  public static void Main(string[] args) {
    #region main
var answer = 42;
Console.WriteLine(answer);
    #endregion
  }
}
".Split('\n')
            };

            Executable result = null;
            CSCompiler.Compile(csSource,
                exe => result = exe,
                errors => {
                    foreach(var err in errors)
                        Console.WriteLine($"{err.Filename} - {err.LineNumber},{err.ColumnNumber}: {err.Description}");
                    Assert.Fail("There should not be any errors!");
                });
            
            Assert.NotNull(result);


            var output = ConsoleOutput.Capture(() => result.Main());
            
            Assert.AreEqual("42", output.Trim());
        }
        
        
        [Test]
        public void Compile_with_errors()
        {
            var csSource = new Sourcecode
            {
                Filename = "program.cs",
                Text = @"using System
public class Program {
  public static void Main(string[] args) {
    #region main
var answer == 42
Console.WriteLine(answer);
    #endregion
  }
}
".Split('\n')
            };

            csrun.data.domain.CompilerError[] result = null;
            CSCompiler.Compile(csSource,
                null,
                errors => result = errors.ToArray());
            
            foreach(var err in result)
                Console.WriteLine($"{err.Filename}-{err.LineNumber},{err.ColumnNumber}: {err.Description}");
            
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(2, result[0].LineNumber);
            Assert.AreEqual(5, result[1].LineNumber);
        }
        
        
        [Test, Explicit]
        public void Spike()
        {
            var csSourcecode = @"using System;
public class Program {
  public static void Main() {
    #region main
var answer = 42;
throw new ApplicationException(""Argh!!!!"");
Console.WriteLine(answer);
Console.WriteLine(Environment.CurrentDirectory);
System.IO.File.ReadAllLines(""xyz.txt"");
    #endregion
  }
}
";
            
            CSharpCodeProvider provider = new CSharpCodeProvider();

            CompilerParameters cp = new CompilerParameters();
            cp.ReferencedAssemblies.Add( "System.dll" );
            cp.GenerateExecutable = true;
            cp.OutputAssembly = Path.Combine(Environment.CurrentDirectory, "program.exe");
            cp.GenerateInMemory = false;
            cp.IncludeDebugInformation = true;
            
            File.WriteAllText("program.cs", csSourcecode);
            CompilerResults cr = provider.CompileAssemblyFromFile(cp, "program.cs");

            if (cr.Errors.HasErrors)
            {
                Console.WriteLine("*** Errors ***");
                foreach (CompilerError ce in cr.Errors)
                {
                    Console.WriteLine($"{Path.GetFileName(ce.FileName)}-{ce.Line},{ce.Column}({ce.IsWarning}): {ce.ErrorText}");
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine($"Compile assembly exists: {cr.CompiledAssembly != null}");

                try
                {
                    var prog = cr.CompiledAssembly;
                    var tProg = prog.GetType("Program");
                    var mMain = tProg.GetMethod("Main");
                    mMain.Invoke(null, new object[0]);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("*** Runtime Exception ***");
                    Console.WriteLine(ex.InnerException);
                    /*
                     * Die Zeilen am Anfang der Exceptions beschreiben den Fehler.
                     * Der StackTrace umfasst alle Zeilen, die danach mit "  at" beginnen.
                     * 
                     * Im Exeption-Text steht in einer Zeile, die mit "  at" beginnt irgendwo am Ende
                     * .../program.cs:<zeilennr>.
                     */
                    Console.WriteLine("***");
                }
            }
        }
    }
}