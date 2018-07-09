using System;

namespace csrun.adapters.providers
{
    internal class FailureLog
    {
        public void DisplayRuntimeFailure(string failure) {
            Console.WriteLine("*** Runtime Exception ***");
            Console.WriteLine(failure);
        }

        public void DisplayCompilerErrors(string[] errors) {
            Console.WriteLine("*** Compiler Errors ***");
            foreach(var err in errors)
                Console.WriteLine($"- {err}");
        }
    }
}