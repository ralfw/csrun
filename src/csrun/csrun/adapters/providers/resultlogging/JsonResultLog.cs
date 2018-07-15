using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Web.Script.Serialization;

namespace csrun.adapters.providers.resultlogging
{
    internal class JsonResultLog : IResultLog {
        private JavaScriptSerializer _json = new JavaScriptSerializer();


        class CompilerErrorDto {
            public readonly string ResultType = "CompilerErrorDto";
            public string[] Errors;
        }
        public void DisplayCompilerErrors(string[] errors) {
            var result = new CompilerErrorDto {Errors = errors};
            var resultJson = _json.Serialize(result);
            Console.WriteLine(resultJson);
        }

        
        class RuntimeFailureDto {
            public readonly string ResultType = "RuntimeFailureDto";
            public string Message;
        }
        public void DisplayRuntimeFailure(string failure) {
            var result = new RuntimeFailureDto {Message = failure};
            var resultJson = _json.Serialize(result);
            Console.WriteLine(resultJson);
        }

        class TestFailureDto {
            public string Label;
            public string Message;
        }
        private List<TestFailureDto> _testFailures = new List<TestFailureDto>();
        public void DisplayTestFailure(string label, string failure) {
            _testFailures.Add(new TestFailureDto{Label = label, Message = failure});
        }

        class TestResultsDto
        {
            public readonly string ResultType = "TestResultsDto";
            public (bool success, string label)[] Results;
            public TestFailureDto[] Failures;
        }
        public void DisplayTestResults((bool success, string label)[] results) {
            var result = new TestResultsDto {
                Results = results,
                Failures = _testFailures.ToArray()
            };
            _testFailures.Clear();
            
            var resultJson = _json.Serialize(result);
            Console.WriteLine(resultJson);
        }
    }
}