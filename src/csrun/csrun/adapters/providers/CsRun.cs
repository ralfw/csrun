using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using csrun.data.dto;
using NUnit.Framework;

namespace csrun.adapters.providers
{
    internal static class CsRun
    {
        public static ResultLogDto Run(string csrunFilename) {
            var output = Run_process(csrunFilename);
            return Deserialize_result(output);
        }
        
        
        static string Run_process(string csrunFilename) {
            var result = new StringBuilder();
            
            var pi = Create_process_info();
            pi.CreateNoWindow = true;
            pi.UseShellExecute = false;
            pi.RedirectStandardOutput = true;

            var p = Process.Start(pi);
            var are = new AutoResetEvent(false);
            
            p.BeginOutputReadLine();
            p.OutputDataReceived += (_, args) => {
                lock (result) {
                    if (args.Data == null)
                        are.Set();
                    else
                        result.AppendLine(args.Data);
                }
            };
            
            p.WaitForExit();
            are.WaitOne(); // final output data might be received after process finished
            p.Close();

            return result.ToString();
                
                
            ProcessStartInfo Create_process_info() {
                var exeFilepath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                return IsRunningOnMono()
                    ? new ProcessStartInfo("mono", $"\"{exeFilepath}\" test -f \"{csrunFilename}\" -json")
                    : new ProcessStartInfo(exeFilepath, $"test -f \"{csrunFilename}\" -json");
                    
                    
                bool IsRunningOnMono() => (Type.GetType("Mono.Runtime") != null);
            }
        }
        
        
        static ResultLogDto Deserialize_result(string result)
        {
            var resultReader = new StringReader(result);
            var resultTypeName = resultReader.ReadLine();
            var resultJson = resultReader.ReadToEnd();
            
            var resultTypes = new Dictionary<string, Type> {
                {typeof(CompilerErrorLogDto).Name, typeof(CompilerErrorLogDto)},
                {typeof(RuntimeFailureLogDto).Name, typeof(RuntimeFailureLogDto)},
                {typeof(TestResultsLogDto).Name, typeof(TestResultsLogDto)}
            };
            var json = new JavaScriptSerializer();
            return (ResultLogDto)json.Deserialize(resultJson, resultTypes[resultTypeName]);
        }
    }
}