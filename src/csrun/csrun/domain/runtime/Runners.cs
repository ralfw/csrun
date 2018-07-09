using System;

namespace csrun.domain.runtime
{
    internal interface IRunner
    {
        void Run(Executable exe, Action<Exception> onException);
    }
    
    
    internal class MainRunner : IRunner
    {
        public void Run(Executable exe, Action<Exception> onException) {
            try {
                exe.Main();
            }
            catch (Exception ex) {
                onException(ex);
            }
        }
    }
}