using System;
using System.Collections.Generic;
using csrun.data.domain;

namespace csrun.domain.runtime
{
    internal interface IRunner {
        IEnumerable<RuntimeResult> Run(Executable exe);
    }
    
    
    internal class MainRunner : IRunner
    {
        public IEnumerable<RuntimeResult> Run(Executable exe) {
            try {
                exe.Main();
                return new[] {new RuntimeSuccess() };
            }
            catch (Exception ex) {
                return new[] {new RuntimeException(ex)};
            }
        }
    }
}