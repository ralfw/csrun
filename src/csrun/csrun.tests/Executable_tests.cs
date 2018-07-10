using System;
using System.Linq;
using csrun.data.domain;
using csrun.domain.compiletime;
using csrun.domain.runtime;
using NUnit.Framework;

namespace csrun.tests
{
    [TestFixture]
    public class Executable_tests
    {
        [Test]
        public void Collect_testmethodnames()
        {
            var csSource = new Sourcecode
            {
                Filename = "program.cs",
                Text = @"using System;
using NUnit.Framework;

public class Program {
  public static void Main(string[] args) {
    #region main
var answer = 42;
Console.WriteLine(answer);
    #endregion
  }

[Test]
public void Test1() {}

public void Dump() {}

[Test]
public void Test2() {}
}
".Split('\n')
            };

            string[] result = null;
            CSCompiler.Compile(csSource,
                exe => result = exe.Testmethodnames.ToArray(),
                null);
            
            Assert.AreEqual(new[]{"Test1", "Test2"}, result);
        }
        
        
        [Test]
        public void Execute_testmethod()
        {
            var csSource = new Sourcecode
            {
                Filename = "program.cs",
                Text = @"using System;
using NUnit.Framework;

public class Program {
  public static void Main(string[] args) {
    #region main
    #endregion
  }

[Test]
public void TestX() { Console.WriteLine(42); }
}
".Split('\n')
            };

            CSCompiler.Compile(csSource,
                exe => {
                    var output = ConsoleOutput.Capture(() => exe.Test("TestX"));
                    Assert.AreEqual("42", output.Trim());
                },
                null);
        }
    }
}