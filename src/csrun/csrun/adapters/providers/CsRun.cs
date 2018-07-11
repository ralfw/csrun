using System;
using System.Diagnostics;

namespace csrun.adapters.providers
{
    internal static class CsRun
    {
        public static void Run(string csrunFilename) {
            var pi = Create_process_info();
            pi.CreateNoWindow = true;
            pi.UseShellExecute = true;

            var p = Process.Start(pi);
            p.WaitForExit();


            ProcessStartInfo Create_process_info()
                => IsRunningOnMono() ? new ProcessStartInfo("mono", $"csrun.exe test -f \"{csrunFilename}\"") 
                                     : new ProcessStartInfo("csrun.exe", $"-f \"{csrunFilename}\"");
            
            bool IsRunningOnMono() => (Type.GetType("Mono.Runtime") != null);
        }
    }
}