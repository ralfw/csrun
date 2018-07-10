﻿using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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

            HandleException(results);
            HandleTestFailures(results);
            HandleTestResults(results);
        }

        void HandleException(RuntimeResult[] results) {
            var runtimeExceptions = results.OfType<RuntimeException>().ToArray();
            if (!runtimeExceptions.Any()) return;
            
            var failure = FailureMapper.MapRuntimeException(runtimeExceptions.First().Exception, _csSource.Text);
            _log.DisplayRuntimeFailure(failure);
        }

        void HandleTestFailures(RuntimeResult[] results) {
            var testFailures = results.OfType<TestFailure>().ToArray();
            if (!testFailures.Any()) return;

            foreach (var tf in testFailures) {
                var failure = FailureMapper.MapRuntimeException(tf.Exception, _csSource.Text);
                _log.DisplayTestFailure(tf.Label, failure);
            }
        }
        
        void HandleTestResults(RuntimeResult[] results)
        {
            var testResults = results.OfType<TestResult>()
                                     .Select(r => ((bool success, string label))(r is TestSuccess, r.Label))
                                     .ToArray();
            if (!testResults.Any()) return;

            _log.DisplayTestResults(testResults);
        }
    }
}