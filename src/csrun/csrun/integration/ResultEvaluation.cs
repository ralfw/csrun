using System.Collections.Generic;
using System.Linq;
using csrun.adapters.providers;
using csrun.data.domain;
using csrun.domain.runtime;

namespace csrun.integration
{
    internal class ResultEvaluation
    {
        private readonly Sourcecode _csSource;
        private readonly FailureLog _log;

        public ResultEvaluation(Sourcecode csSource, FailureLog log) {
            _csSource = csSource;
            _log = log;
        }

        
        public void HandleCompilerErrors(IEnumerable<CompilerError> compilerErrors) {
            var errors = FailureMapper.MapCompiletimeErrors(compilerErrors, _csSource.Text);
            _log.DisplayCompilerErrors(errors);
        }
        
        
        public void HandleRuntimeResults(RuntimeResult[] results) {
            if (!results.Any()) return;
            if (results.First() is RuntimeSuccess) return;

            HandleException(results, _csSource);
        }

        void HandleException(RuntimeResult[] results, Sourcecode csSource) {
            var runtimeEx = results.FirstOrDefault(r => r is RuntimeException);
            if (runtimeEx == null) return;
            
            var failure = FailureMapper.MapRuntimeException(results.First(), csSource.Text);
            _log.DisplayRuntimeFailure(failure);
        }
    }
}