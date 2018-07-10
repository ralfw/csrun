using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using csrun.adapters.providers;
using csrun.data.domain;
using csrun.domain.compiletime;
using csrun.domain.runtime;

namespace csrun
{
    internal class App
    {
        private readonly Filesystem _fs;
        private readonly FailureLog _failureLog;
        private readonly CLI.Command _cmd;

        public App(Filesystem fs, FailureLog failureLog, CLI.Command cmd) {
            _fs = fs;
            _failureLog = failureLog;
            _cmd = cmd;
        }
        
        
        public void Execute() {
            var csSource = Transpile(_cmd.SourceFilename);
            Compile(csSource,
                exe => Run(exe, csSource)
            );
        }

        
        Sourcecode Transpile(string csrunFilename) {
            var csrunSource = _fs.ReadSource(csrunFilename);
            var csrunSections = Dissection.Dissect(csrunSource);
            var csTemplate = _fs.LoadTemplate();
            return Rendering.Render(csrunSections.ToArray(), csTemplate);
        }

        
        void Compile(Sourcecode csSource, Action<Executable> onSuccess) {
            CSCompiler.Compile(csSource,
                onSuccess,
                onFailure: compilerErrors => {
                    var errors = FailureMapper.MapCompiletimeErrors(compilerErrors, csSource.Text);
                    _failureLog.DisplayCompilerErrors(errors);
                });
        }

        
        void Run(Executable exe, Sourcecode csSource) {
            var runner = SelectRunner(_cmd);
            var errors = runner.Run(exe);
            HandleRuntimeErrors(errors.ToArray(), csSource);
        }
        
        private IRunner SelectRunner(CLI.Command cmd) {
            switch (cmd) {
                case CLI.TestCommand _:
                case CLI.WatchCommand _:
                case CLI.RunCommand _:
                default: return new MainRunner();
            }
        }
        
        void HandleRuntimeErrors(RuntimeError[] errors, Sourcecode csSource) {
            if (!errors.Any()) return;
            
            var failure = FailureMapper.MapRuntimeException(errors.First(), csSource.Text);
            _failureLog.DisplayRuntimeFailure(failure);
        }
    }
}

