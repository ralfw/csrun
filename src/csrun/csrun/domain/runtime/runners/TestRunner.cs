using System;
using System.Collections.Generic;
using csrun.data.domain;

namespace csrun.domain.runtime.runners
{
    internal class TestRunner : IRunner
    {
        public IEnumerable<RuntimeResult> Run(Executable exe)
        {
            var results = new List<TestResult>();
            foreach (var method in exe.Testmethods) {
                try {
                    exe.Test(method.name);
                    results.Add(new TestSuccess(method.label));
                }
                catch (Exception ex) {
                    results.Add(new TestFailure(method.label, ex));
                }
            }
            return results;
        }
    }
}