﻿using System;
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

        public App(Filesystem fs, FailureLog failureLog) {
            _fs = fs;
            _failureLog = failureLog;
        }
        
        
        public void Execute(CLI.Command cmd) {
            var csSource = Transpile(cmd.SourceFilename);
            Compile(csSource,
                exe => {
                    var runner = SelectRunner(cmd);
                    var errors = runner.Run(exe);
                    HandleRuntimeErrors(errors.ToArray(), csSource);
                }
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

        void HandleRuntimeErrors(RuntimeError[] errors, Sourcecode csSource) {
            if (!errors.Any()) return;
            
            var failure = FailureMapper.MapRuntimeException(errors.First(), csSource.Text);
            _failureLog.DisplayRuntimeFailure(failure);
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

