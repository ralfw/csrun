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
            if (args.Length == 1 && Path.GetExtension(args[0])?.ToLower() == ".csrun")
                return new RunCommand(args[0]);
            
            var schema = new appcfg.AppCfgSchema("csrun.exe.config.json", 
                new Route("run", isDefault:true)
                    .Param("sourceFilename", "s,f,source,filename", ValueTypes.String, isRequired:true),
                new Route("test")
                    .Param("sourceFilename", "s,f,source,filename", ValueTypes.String, isRequired:true),
                new Route("watch")
                    .Param("sourceFilename", "s,f,source,filename", ValueTypes.String, isRequired:true));
            var cfg = new AppCfgCompiler(schema)
                            .Compile(args);

            switch (cfg._RoutePath) {
                case "run": return new RunCommand(cfg.sourceFilename);
                case "test": return new TestCommand(cfg.sourceFilename);
                case "watch": return new WatchCommand(cfg.sourceFilename);
                default: throw new ApplicationException($"Unknown command: '{cfg._RoutePath}'!");
            }
        }


        public class Command {
            protected Command(string sourceFilename) {
                if (Path.GetExtension(sourceFilename)?.ToLower() != ".csrun") 
                    throw new ApplicationException($"Source filename has invalid extension '{Path.GetExtension(sourceFilename)}'! '.csrun' required!");
                
                this.SourceFilename = sourceFilename;
            }
            
            public string SourceFilename { get; }
        }

        public class RunCommand : Command {
            internal RunCommand(string sourceFilename) : base(sourceFilename) {}
        }
        
        public class TestCommand : Command {
            internal TestCommand(string sourceFilename) : base(sourceFilename) {}
        }
        
        public class WatchCommand : Command {
            internal WatchCommand(string sourceFilename) : base(sourceFilename) {}
        }
    }
}