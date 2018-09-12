using System;
using System.IO;
using appcfg;

namespace csrun.adapters.providers
{
    /*
     * csrun.exe fibonacci.csrun // like with "run" route path
     * csrun.exe [ run ] -source fibonacci.csrun  // no tests executed
     * csrun.exe test -source fib.csrun // only tests executed once
     * csrun.exe watch -source fibonacci.csrun // only tests executed continuously
     */
    internal class CLI
    {
        public static Command Parse(string[] args)
        {
            const string DEFAULT_TEMPLATE_FILENAME = "template.cs";
            
            if (args.Length == 1 && Path.GetExtension(args[0])?.ToLower() == ".csrun")
                return new RunCommand(args[0],DEFAULT_TEMPLATE_FILENAME);
            
            var schema = new appcfg.AppCfgSchema("csrun.exe.config.json", 
                new Route("run", isDefault:true)
                    .Param("sourceFilename", "s,f,source,filename", ValueTypes.String, isRequired:true)
                    .Param("templateFilename", "t,template", ValueTypes.String, defaultValue: DEFAULT_TEMPLATE_FILENAME),
                new Route("test")
                    .Param("sourceFilename", "s,f,source,filename", ValueTypes.String, isRequired:true)
                    .Param("jsonOutput", "json")
                    .Param("templateFilename", "t,template", ValueTypes.String, defaultValue: DEFAULT_TEMPLATE_FILENAME),
                new Route("watch")
                    .Param("sourceFilename", "s,f,source,filename", ValueTypes.String, isRequired:true)
                    .Param("templateFilename", "t,template", ValueTypes.String, defaultValue: DEFAULT_TEMPLATE_FILENAME));

            try
            {
                var cfg = new AppCfgCompiler(schema)
                    .Compile(args);

                switch (cfg._RoutePath)
                {
                    case "run": return new RunCommand(cfg.sourceFilename, cfg.templateFilename);
                    case "test": return new TestCommand(cfg.sourceFilename, cfg.jsonOutput, cfg.templateFilename);
                    case "watch": return new WatchCommand(cfg.sourceFilename, cfg.templateFilename);
                    default: throw new ApplicationException($"Unknown command: '{cfg._RoutePath}'!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*** Error found in command line: {0}", ex.Message);
                Console.WriteLine(@"
Usage:

csrun.exe <source filename> // run main section
csrun.exe -f <source filename> [ -t <template filename> ]
csrun.exe test -f <source filename> [ -json ]  [ -t <template filename> ] // run tests
csrun.exe watch -f <source filename>  [ -t <template filename> ] // run tests whenever the source file changes
");
                Environment.Exit(1);
                throw new ApplicationException("this is never reached. it just soothes the compiler");
            }
        }


        public class Command {
            protected Command(string sourceFilename, string templateFilename) {
                if (Path.GetExtension(sourceFilename)?.ToLower() != ".csrun") 
                    throw new ApplicationException($"Source filename has invalid extension '{Path.GetExtension(sourceFilename)}'! '.csrun' required!");
                
                this.SourceFilename = sourceFilename;
                this.TemplateFilename = templateFilename;
            }
            
            public string SourceFilename { get; }
            public string TemplateFilename { get; }
        }

        public class RunCommand : Command {
            internal RunCommand(string sourceFilename, string templateFilename) : base(sourceFilename, templateFilename) {}
        }
        
        public class TestCommand : Command
        {
            internal TestCommand(string sourceFilename, bool jsonOutput, string templateFilename) : base(sourceFilename, templateFilename) {
                this.JsonOutput = jsonOutput;
            }
            
            public bool JsonOutput { get; }
        }

        public class WatchCommand : Command {
            internal WatchCommand(string sourceFilename, string templateFilename) : base(sourceFilename, templateFilename) {}
        }
    }
}