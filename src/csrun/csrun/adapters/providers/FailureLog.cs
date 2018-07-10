﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace csrun.adapters.providers
{
    internal class FailureLog
    {
        public void DisplayCompilerErrors(string[] errors) {
            Console.WriteLine("*** Compiler Errors ***");
            foreach(var err in errors)
                Console.WriteLine($"- {err}");
        }
        
        public void DisplayRuntimeFailure(string failure) {
            Console.WriteLine("*** Runtime Exception ***");
            Console.WriteLine(failure);
        }
        
        public void DisplayTestResults((bool success, string label)[] results) {
            Console.WriteLine("*** Overall Test Results ***");
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
        
        public void DisplayTestFailure(string label, string failure) {
            Console.WriteLine($"*** Failed Test '{label}' ***");
            Console.WriteLine(failure);
        }
    }
}