using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using csrun.data.domain;

namespace csrun.domain.runtime
{
    internal interface IRunner {
        IEnumerable<RuntimeResult> Run(Executable exe);
    }
    
    
    internal class MainRunner : IRunner
    {
        public IEnumerable<RuntimeResult> Run(Executable exe) {
            try {
                exe.Main();
                return new[] {new RuntimeSuccess() };
            }
            catch (Exception ex) {
                return new[] {new RuntimeException(ex)};
            }
        }
    }
    
    
    internal class TestRunner : IRunner
    {
        public IEnumerable<RuntimeResult> Run(Executable exe)
        {
            var results = new List<TestResult>();
            foreach (var name in exe.Testmethodnames)
            {
                var label = TestMethodName.ExtractLabel(name);
                try {
                    exe.Test(name);
                    results.Add(new TestSuccess(label));
                }
                catch (Exception ex) {
                    results.Add(new TestFailure(label, ex));
                }
            }
            return results;
        }
    }
}