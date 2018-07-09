using System;
using csrun.adapters.providers;
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
        public void Test()
        {
            var fs = new Filesystem();
            var fl = new FailureLog();
            var sut = new App(fs, fl);
            
            sut.Execute(new CLI.RunCommand("example.csrun"));
        }
    }
}