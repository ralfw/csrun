using System.IO;
using csrun.adapters.providers;
using csrun.integration;

namespace csrun.adapters.controllers
{
    /*
     * csrun.exe can be moved into a folder beneath the soure directory, e.g. .csrun/
     * It can then be started from the parent/source folder like this: .csrun/csrun.exe
     *
     * If the .csrun-source is named example.csrun then the rendered source will
     * be named example.cs and the resulting assembly example.exe.
     * These files will be put next to the source.
     * The generated .exe can be started individually.
     */
    internal class Program
    {
        public static void Main(string[] args)
        {
            var cmd = CLI.Parse(args);
            
            var fs = new Filesystem();
            var ui = new FailureLog();
            var app = new App(fs, ui, cmd);
            
            app.Execute();
        }
    }
}