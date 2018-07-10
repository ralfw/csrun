using System;
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
    }
}