using System;
using System.Collections.Generic;

namespace csrun.domain.runtime
{
    internal interface IRunner {
        IEnumerable<RuntimeError> Run(Executable exe);
    }
    
    
    internal class MainRunner : IRunner
    {
        public IEnumerable<RuntimeError> Run(Executable exe) {
            try {
                exe.Main();
                return new RuntimeError[0];
            }
            catch (Exception ex) {
                return new[] {new RuntimeException(ex)};
            }
        }
    }
}