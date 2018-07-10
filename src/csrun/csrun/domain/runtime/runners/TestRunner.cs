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