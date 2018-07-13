using System;
using System.Diagnostics;

namespace testparent
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Parent running...");
            var pi = Create_process_info();
            pi.CreateNoWindow = true;
            pi.UseShellExecute = false;
            
            Console.WriteLine($"-> {pi.FileName} / {pi.Arguments}");

            var p = new Process {StartInfo = pi};
            p.Start();
            p.WaitForExit();
            Console.WriteLine("Parent finished");

            ProcessStartInfo Create_process_info() {
                var exeFilepath = "testchild.exe";
                return IsRunningOnMono()
                    ? new ProcessStartInfo("mono", $"\"{exeFilepath}\"")
                    : new ProcessStartInfo(exeFilepath, "");
            }
            
            bool IsRunningOnMono() => (Type.GetType("Mono.Runtime") != null);
        }
    }
}