﻿using System;
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
                Filename = "program_collecttestmethodnames.cs",
                Text = @"using System;
using NUnit.Framework;

public class Program {
  public static void Main(string[] args) {
    #region main
var answer = 42;
Console.WriteLine(answer);
    #endregion
  }

[Test(Description='Test 1!')]
public void Test1() {}

public void Dump() {}

[Test()]
public void Test2() {}
}
".Replace("'", "\"").Split('\n')
            };

            (string name,string label)[] result = null;
            CSCompiler.Compile(csSource,
                exe => result = exe.Testmethods.ToArray(),
                null);
            
            Assert.AreEqual(new[]{"Test1", "Test2"}, result.Select(r => r.name));
            Assert.AreEqual(new[]{"Test 1!", "[no name]"}, result.Select(r => r.label));
        }
        
        
        [Test]
        public void Execute_calltestmethod()
        {
            var csSource = new Sourcecode
            {
                Filename = "program_calltestmethod.cs",
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