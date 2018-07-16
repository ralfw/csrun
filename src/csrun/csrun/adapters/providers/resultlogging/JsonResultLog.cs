using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Web.Script.Serialization;
using csrun.data.dto;

namespace csrun.adapters.providers.resultlogging
{
    internal class JsonResultLog : IResultLog {
        private readonly JavaScriptSerializer _json = new JavaScriptSerializer();

        
        public void DisplayCompilerErrors(string[] errors) {
            var result = new CompilerErrorLogDto {Errors = errors};
            var resultJson = _json.Serialize(result);
            Console.WriteLine(result.GetType().Name);
            Console.WriteLine(resultJson);
        }

        

        public void DisplayRuntimeFailure(string failure) {
            var result = new RuntimeFailureLogDto {Message = failure};
            var resultJson = _json.Serialize(result);
            Console.WriteLine(result.GetType().Name);
            Console.WriteLine(resultJson);
        }


        private readonly List<TestResultsLogDto.TestFailureDto> _testFailures = new List<TestResultsLogDto.TestFailureDto>();
        public void DisplayTestFailure(string label, string failure) {
            _testFailures.Add(new TestResultsLogDto.TestFailureDto{Label = label, Message = failure});
        }


        public void DisplayTestResults((bool success, string label)[] results) {
            var result = new TestResultsLogDto {
                Results = results,
                Failures = _testFailures.ToArray()
            };
            _testFailures.Clear();
            
            var resultJson = _json.Serialize(result);
            Console.WriteLine(result.GetType().Name);
            Console.WriteLine(resultJson);
        }
    }
}