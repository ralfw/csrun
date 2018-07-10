using System;

namespace csrun.domain.runtime
{
    class RuntimeResult {}
    
    class RuntimeException : RuntimeResult {
        private readonly Exception _ex;
        public RuntimeException(Exception ex) { _ex = ex; }

        public override string ToString() {
            return _ex.ToString();
        }
    }

    class TestFailure : RuntimeResult {
        private readonly string _label;
        private readonly Exception _ex;

        public TestFailure(string label, Exception ex) {
            _label = label;
            _ex = ex;
        }

        public override string ToString() {
            return $"Test failed: '{_label}': {_ex}";
        }
    }

    class TestSuccess : RuntimeResult
    {
        
    }
}


