using System;
using System.IO;
using csrun.adapters.providers;
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
            var fl = new FailureLog();
            var cmd = new CLI.RunCommand("test_addition.csrun");
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
            var fl = new FailureLog();
            var cmd = new CLI.TestCommand("test_tests.csrun");
            var sut = new App(fs, fl, cmd);

            var output = ConsoleOutput.Capture(() => sut.Execute());

            Console.WriteLine(output);
        }
        
        
        
        [Test]
        public void Change_source_and_reexecute()
        {
            const string CSRUN_FILENAME = "test.csrun";
            
            var source1 = "Console.WriteLine(\"123\");";
            var source2 = "Console.WriteLine(\"987\");";
            
            var fs = new Filesystem();
            var fl = new FailureLog();
            var cmd = new CLI.RunCommand(CSRUN_FILENAME);
            var sut = new App(fs, fl, cmd);
            
            File.WriteAllText(CSRUN_FILENAME, source1);
            var output = ConsoleOutput.Capture(() => sut.Execute());
            Assert.AreEqual("123", output.Trim());
            
            File.WriteAllText(CSRUN_FILENAME, source2);
            output = ConsoleOutput.Capture(() => sut.Execute());
            Assert.AreEqual("987", output.Trim());
        }
    }
}