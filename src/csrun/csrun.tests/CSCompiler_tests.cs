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
                Filename = "program_no_errors.cs",
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
                Filename = "program_with_errors.cs",
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
    }
}