using System.Linq;
using csrun.adapters.providers;
using csrun.adapters.providers.resultlogging;
using csrun.data.domain;
using csrun.domain.compiletime;
using csrun.domain.runtime;
using csrun.domain.runtime.runners;

namespace csrun.integration
{
    internal class App
    {
        private readonly Filesystem _fs;
        private readonly IResultLog _resultLog;
        private readonly CLI.Command _cmd;
        private ResultEvaluation _reval;

        public App(Filesystem fs, IResultLog resultLog, CLI.Command cmd) {
            _fs = fs;
            _resultLog = resultLog;
            _cmd = cmd;
        }


        public void Execute() {
            if (_cmd is CLI.WatchCommand)
                ExecuteOnChange();
            else
                ExecuteOnce();
        }


        private void ExecuteOnChange() {
            var timer = new BlockingTimer();
            var observer = new SourceObservation(_fs, timer);
            observer.WatchForChange(_cmd.SourceFilename,
                () => CsRun.Run(_cmd.SourceFilename)
            );
        }
        
        
        private void ExecuteOnce() {
            var csSource = Transpile(_cmd.SourceFilename);
            _reval = new ResultEvaluation(csSource, _resultLog);
            CSCompiler.Compile(csSource,
                onSuccess: Run,
                onFailure: _reval.HandleCompilerErrors
            );
        }

        
        Sourcecode Transpile(string csrunFilename) {
            var csrunSource = _fs.ReadSource(csrunFilename);
            var csrunSections = Dissection.Dissect(csrunSource);
            var csTemplate = _fs.LoadTemplate();
            return Rendering.Render(csrunSections.ToArray(), csTemplate);
        }

        
        void Run(Executable exe) {
            var results = SelectRunner().Run(exe);
            _reval.HandleRuntimeResults(results.ToArray());
            
            
            IRunner SelectRunner() {
                switch (_cmd) {
                    case CLI.TestCommand _:
                    case CLI.WatchCommand _:
                        return new TestRunner();
                    default: return new MainRunner();
                }
            }
        }
    }
}

