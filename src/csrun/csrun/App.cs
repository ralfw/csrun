using System.Collections;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using csrun.adapters.providers;
using csrun.domain.compiletime;
using csrun.domain.runtime;

namespace csrun
{
    internal class App
    {
        private readonly Filesystem _fs;
        private readonly FailureLog _failureLog;

        public App(Filesystem fs, FailureLog failureLog) {
            _fs = fs;
            _failureLog = failureLog;
        }
        
        
        public void Execute(CLI.Command cmd) {
            var csrunSource = _fs.ReadSource(cmd.SourceFilename);
            var csrunSections = Dissection.Dissect(csrunSource);
            var csTemplate = _fs.LoadTemplate();
            var csSource = Rendering.Render(csrunSections.ToArray(), csTemplate);
            var fp = new FailureMapper(csSource.Text);
            CSCompiler.Compile(csSource,
                onSuccess: exe => {
                    var runner = SelectRunner(cmd);
                    runner.Run(exe,
                        onException: ex => {
                            var failure = fp.MapRuntimeException(ex);
                            _failureLog.DisplayRuntimeFailure(failure);
                        });
                },
                onFailure: compilerErrors => {
                    var errors = fp.MapCompiletimeErrors(compilerErrors);
                    _failureLog.DisplayCompilerErrors(errors);
                });
        }


        private IRunner SelectRunner(CLI.Command cmd) {
            switch (cmd) {
                case CLI.TestCommand _:
                case CLI.WatchCommand _:
                case CLI.RunCommand _:
                default: return new MainRunner();
            }
        }
    }
}

