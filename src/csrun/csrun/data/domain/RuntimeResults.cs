using System;
using System.Runtime.InteropServices;

namespace csrun.data.domain
{
    class RuntimeResult {}
    
    
    class RuntimeException : RuntimeResult {
        public RuntimeException(Exception ex) { Exception = ex; }

        public Exception Exception { get; }
    }
    
    class RuntimeSuccess : RuntimeResult {}


    class TestResult : RuntimeResult {
        protected TestResult(string label) {
            Label = label;
        }
        public string Label { get; }
    }
    
    class TestFailure : TestResult {
        public TestFailure(string label, Exception ex) : base(label) {
            Exception = ex;
        }

        public Exception Exception { get; }
    }

    class TestSuccess : TestResult {
        public TestSuccess(string label) : base(label) {}
    }
}


