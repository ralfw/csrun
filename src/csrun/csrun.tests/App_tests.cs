using System;
using System.Collections.Generic;
using System.IO;
using csrun.adapters.providers;
using csrun.adapters.providers.resultlogging;
using csrun.integration;
using NUnit.Framework;

namespace csrun.tests
{
    [TestFixture]
    public class App_tests
    {
        [SetUp]
        public void Setup() {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
        }
        
        
        [Test]
        public void Test_functions()
        {
            var fs = new Filesystem();
            var fl = new TextResultLog();
            var cmd = new CLI.RunCommand("App_tests_with_addition.csrun");
            var sut = new App(fs, fl, cmd);
            
            var output = ConsoleOutput.Capture(() => sut.Execute());
            
            Assert.AreEqual("1+2=3", output.Trim());
        }
        
        [Test, Explicit]
        /*
         * This test fails surprisingly, even though it just outputs a string.
         * The exceptions thrown by the assert inside the tests which get executed
         * are caught. Nevertheless at least JetBrains Rider shows this test failing.
         */
        public void Test_tests()
        {
            var fs = new Filesystem();
            var fl = new TextResultLog();
            var cmd = new CLI.TestCommand("App_tests_with_failure.csrun", false);
            var sut = new App(fs, fl, cmd);

            var output = ConsoleOutput.Capture(() => sut.Execute());

            Console.WriteLine(output);
        }        
        
        [Test, Explicit]
        public void Test_tests_with_json_outptu()
        {
            var fs = new Filesystem();
            var fl = new JsonResultLog();
            var cmd = new CLI.TestCommand("App_tests_with_failure.csrun", true);
            var sut = new App(fs, fl, cmd);

            var output = ConsoleOutput.Capture(() => sut.Execute());

            Console.WriteLine(output);
        }
        
        
        [Test]
        // The solution for this lies in TestRunner{}
        public void Bugfix_Test_errors_dont_get_accumulate_by_NUnit()
        {
            const string CSRUN_FILENAME = "test.csrun";
            
            var csrunText = @"#test failing 1
Assert.IsTrue(false);
#test failing 2
Assert.AreEqual(42,0);";
            File.WriteAllText(CSRUN_FILENAME, csrunText);

            var mockResultLog = new MockResultLog();
            var sut = new App(new Filesystem(), mockResultLog, new CLI.TestCommand(CSRUN_FILENAME, false));

            sut.Execute();

            foreach(var f in mockResultLog.TestFailures)
                Console.WriteLine($"<<<{f}>>>");

            Assert.AreEqual(2, mockResultLog.TestFailures.Count);
            Assert.IsTrue(mockResultLog.TestFailures[0].IndexOf("Multiple failures") < 0);
            Assert.IsTrue(mockResultLog.TestFailures[1].IndexOf("Multiple failures") < 0);
        }


        class MockResultLog : IResultLog
        {
            public List<string> TestFailures = new List<string>();
            
            public void DisplayTestFailure(string label, string failure) {
                this.TestFailures.Add($"{label}:{failure}");
            }

            public void DisplayCompilerErrors(string[] errors) {
                throw new System.NotImplementedException();
            }

            public void DisplayRuntimeFailure(string failure) {
                throw new System.NotImplementedException();
            }

            public void DisplayTestResults((bool success, string label)[] results) {}
        }
    }
}