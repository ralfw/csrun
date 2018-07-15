using System;
using System.Diagnostics;
using System.Text;

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
            pi.RedirectStandardOutput = true;
            
            Console.WriteLine($"-> {pi.FileName} / {pi.Arguments}");

            var p = new Process {StartInfo = pi};
            p.EnableRaisingEvents = true;
            bool exited = false;
            p.Exited += (a,b) => exited = true;
            
            p.Start();
            var output = p.StandardOutput;

            var sb = new StringBuilder();
            var buff = new char[1024];
            while (!exited) {
                var length = p.StandardOutput.Read(buff, 0, buff.Length);
                sb.Append(buff.SubArray(0, length));
            }
            Console.WriteLine("Parent finished");
            Console.WriteLine($"<{sb}>");

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