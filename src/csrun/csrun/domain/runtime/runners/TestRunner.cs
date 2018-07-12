using System;
using System.Collections.Generic;
using csrun.data.domain;
using NUnit.Framework;

namespace csrun.domain.runtime.runners
{
    internal class TestRunner : IRunner
    {
        public IEnumerable<RuntimeResult> Run(Executable exe)
        {
            var results = new List<TestResult>();
            foreach (var method in exe.Testmethods) {
                using (new NUnit.Framework.Internal.TestExecutionContext.IsolatedContext()) {
                    // Creating a TestExecutionContext isolates the results of the asserts in one test
                    // from those in another tests. Otherwise NUnit accumulates them and carries
                    // results over to the next Assert call.
                    try {
                        exe.Test(method.name);
                        results.Add(new TestSuccess(method.label));
                    }
                    catch (Exception ex) {
                        results.Add(new TestFailure(method.label, ex));
                    }
                }
            }
            return results;
        }
    }
}