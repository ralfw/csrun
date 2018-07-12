using System;
using System.Threading;

namespace csrun.adapters.providers
{
    internal class BlockingTimer : IDisposable
    {
        private const int DUETIME_MS = 0;
        private const int INTERVAL_MS = 1000;
        
        private Timer _timer;
        private AutoResetEvent _are;
        
        
        public void Start(Action onTick) {
            var busy = false;
            _timer = new Timer(_ => {
                if (busy) return;
                busy = true;
                
                onTick();
                
                busy = false;
            }, null, DUETIME_MS, INTERVAL_MS);
            _are = new AutoResetEvent(false);
        }

        
        public void Wait() => _are?.WaitOne();


        private void Stop() {
            _timer?.Change(Timeout.Infinite, Timeout.Infinite);
            _are?.Set();
        }

        
        public void Dispose() => Stop();
    }
}