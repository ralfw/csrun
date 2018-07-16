using System;
using csrun.adapters.providers;
using csrun.data.dto;
using NUnit.Framework;

namespace csrun.tests
{
    [TestFixture]
    public class CsRun_tests
    {
        [SetUp]
        public void Setup() => Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
        
        
        [Test]
        public void Receive_test_results() {
            var result = CsRun.Run("CsRun_tests_with_failure.csrun");
            switch (result) {
                case CompilerErrorLogDto cel:
                case RuntimeFailureLogDto rfl:
                    Assert.Fail($"Unexpected result type: {result.GetType().Name}");
                    break;
                case TestResultsLogDto trl:
                    Assert.AreEqual(3, trl.Results.Length);
                    Assert.IsFalse(trl.Results[0].success);
                    Assert.AreEqual("fehlschlagender test", trl.Results[0].label);
                    Assert.IsTrue(trl.Results[1].success);
                    Assert.AreEqual("addition mit positiven zahlen", trl.Results[2].label);
                    
                    Assert.AreEqual(1, trl.Failures.Length);
                    Assert.AreEqual("fehlschlagender test", trl.Failures[0].Label);
                    break;
            }
        }
        
        
        [Test]
        public void Receive_compiler_error() {
            var result = CsRun.Run("CsRun_tests_with_compiler_error.csrun");
            switch (result) {
                case CompilerErrorLogDto cel:
                    foreach(var e in cel.Errors)
                        Console.WriteLine($"{e}");
                    Assert.AreEqual(2, cel.Errors.Length);
                    Assert.IsTrue(cel.Errors[0].IndexOf("Unexpected symbol `return'")>=0);
                    Assert.IsTrue(cel.Errors[1].IndexOf("Unexpected symbol `]'")>=0);
                    break;
                case RuntimeFailureLogDto rfl:
                case TestResultsLogDto trl:
                    Assert.Fail($"Unexpected result type: {result.GetType().Name}");
                    break;
            }
        }
    }
}