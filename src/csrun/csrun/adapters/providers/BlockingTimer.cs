using System;
using System.Threading;

namespace csrun.adapters.providers
{
    internal class BlockingTimer : IDisposable
    {
        private const int DUETIME_MS = 0;
        private const int DEFAULT_INTERVAL_MS = 500;

        private int _intervalMs;
        private Timer _timer;
        private AutoResetEvent _are;


        public BlockingTimer() : this(DEFAULT_INTERVAL_MS) {}
        internal BlockingTimer(int intervalMs) {
            _intervalMs = intervalMs;
        }
        
        
        public void Start(Action onTick) {
            var busy = false;
            _timer = new Timer(_ => {
                if (busy) return;
                busy = true;
                
                onTick();
                
                busy = false;
            }, null, DUETIME_MS, _intervalMs);
            _are = new AutoResetEvent(false);
        }

        
        public void Wait() => _are?.WaitOne();


        public void Stop() {
            _timer?.Change(Timeout.Infinite, Timeout.Infinite);
            _are?.Set();
        }

        
        public void Dispose() => Stop();
    }
}