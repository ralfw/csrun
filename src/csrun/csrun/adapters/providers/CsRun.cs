using System;
using System.Diagnostics;
using System.IO;

namespace csrun.adapters.providers
{
    internal static class CsRun
    {
        public static void Run(string csrunFilename) {
            var pi = Create_process_info();
            pi.CreateNoWindow = true;
            pi.UseShellExecute = false;

            var p = Process.Start(pi);
            p.WaitForExit();


            ProcessStartInfo Create_process_info() {
                var exeFilepath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                return IsRunningOnMono()
                    ? new ProcessStartInfo("mono", $"\"{exeFilepath}\" test -f \"{csrunFilename}\"")
                    : new ProcessStartInfo(exeFilepath, $"test -f \"{csrunFilename}\"");
            }

            bool IsRunningOnMono() => (Type.GetType("Mono.Runtime") != null);
        }
    }
}