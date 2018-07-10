using System;
using System.Collections.Generic;

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
                return new RuntimeResult[0];
            }
            catch (Exception ex) {
                return new[] {new RuntimeException(ex)};
            }
        }
    }
}