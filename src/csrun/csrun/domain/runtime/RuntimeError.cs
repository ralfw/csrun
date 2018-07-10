using System;

namespace csrun.domain.runtime
{
    class RuntimeError {}
    
    class RuntimeException : RuntimeError {
        private readonly Exception _ex;
        public RuntimeException(Exception ex) { _ex = ex; }

        public override string ToString() {
            return _ex.ToString();
        }
    }

    class TestFailure : RuntimeError {
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
}


