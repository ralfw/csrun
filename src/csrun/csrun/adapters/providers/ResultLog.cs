using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace csrun.adapters.providers
{
    internal interface IResultLog {
        void DisplayCompilerErrors(string[] errors);
        void DisplayRuntimeFailure(string failure);
        void DisplayTestFailure(string label, string failure);
        void DisplayTestResults((bool success, string label)[] results);
    }

    
    internal class ResultLog : IResultLog
    {
        public void DisplayCompilerErrors(string[] errors) {
            Console.WriteLine("\n*** Compiler Errors ***");
            foreach(var err in errors)
                Console.WriteLine($"- {err}");
        }
        
        public void DisplayRuntimeFailure(string failure) {
            Console.WriteLine("\n*** Runtime Exception ***");
            Console.WriteLine(failure);
        }
        
        public void DisplayTestFailure(string label, string failure) {
            Console.WriteLine($"\n*** Failed Test '{label}' ***");
            Console.WriteLine(failure.Trim());
        }

        public void DisplayTestResults((bool success, string label)[] results) {
            Console.WriteLine($"\n*** Overall Test Results {DateTime.Now} ***");
            var succeeded = results.Count(r => r.success);
            
            var currColor = Console.BackgroundColor;
            foreach (var r in results) {
                Console.BackgroundColor = r.success ? ConsoleColor.Green : ConsoleColor.Red;
                var bulletpoint = r.success ? '+' : '-';
                Console.Write($"{bulletpoint} {r.label}");
                Console.BackgroundColor = currColor;
                Console.WriteLine();
            }
            
            Console.WriteLine($"= {results.Length} tests, {succeeded} succeeded, {results.Length-succeeded} failed");
        }
    }
}