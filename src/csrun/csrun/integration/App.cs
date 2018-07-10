using System.Linq;
using csrun.adapters.providers;
using csrun.data.domain;
using csrun.domain.compiletime;
using csrun.domain.runtime;

namespace csrun.integration
{
    internal class App
    {
        private readonly Filesystem _fs;
        private readonly FailureLog _failureLog;
        private readonly CLI.Command _cmd;
        private ResultEvaluation _reval;
        private readonly IRunner _runner;

        public App(Filesystem fs, FailureLog failureLog, CLI.Command cmd) {
            _fs = fs;
            _failureLog = failureLog;
            _cmd = cmd;
            _runner = SelectRunner();
            
            IRunner SelectRunner() {
                switch (cmd) {
                    case CLI.TestCommand _:
                    case CLI.WatchCommand _:
                    case CLI.RunCommand _:
                    default: return new MainRunner();
                }
            }
        }


        public void Execute() {
            var csSource = Transpile(_cmd.SourceFilename);
            _reval = new ResultEvaluation(csSource, _failureLog);
            CSCompiler.Compile(csSource,
                Run,
                _reval.HandleCompilerErrors
            );
        }

        
        Sourcecode Transpile(string csrunFilename) {
            var csrunSource = _fs.ReadSource(csrunFilename);
            var csrunSections = Dissection.Dissect(csrunSource);
            var csTemplate = _fs.LoadTemplate();
            return Rendering.Render(csrunSections.ToArray(), csTemplate);
        }

        
        void Run(Executable exe) {
            var results = _runner.Run(exe);
            _reval.HandleRuntimeResults(results.ToArray());
        }
    }
}

