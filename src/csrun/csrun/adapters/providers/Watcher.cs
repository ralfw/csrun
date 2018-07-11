using System;
using System.IO;
using System.Threading;

namespace csrun.adapters.providers
{
    internal class Watcher
    {
        private const int DUETIME_MS = 0;
        private const int INTERVAL_MS = 1000;
        
        private readonly string _filename;
        private Timer _timer;
        private AutoResetEvent _are;
        
        public Watcher(string filename) {
            _filename = filename;
        }

        
        public void Start(Action onChanged) {
            Console.WriteLine($"Started watching {_filename}...");
            
            Run();
            Poll(
                Run);

            
            void Run() {
                onChanged();
                Console.WriteLine($"\nAbort watch mode by pressing Ctrl-C");
            }
        }

        
        private void Poll(Action onChanged)
        {
            var busy = false;
            var lastChanged = File.GetLastWriteTime(_filename);
            _timer = new Timer(_ => {
                if (busy) return;
                
                var currentTimestamp = File.GetLastWriteTime(_filename);
                if (currentTimestamp <= lastChanged) return;
                
                lastChanged = currentTimestamp;
                busy = true;

                onChanged();
                
                busy = false;
            }, null, DUETIME_MS, INTERVAL_MS);
            
            _are = new AutoResetEvent(false);
            _are.WaitOne();
        }
        
        
        public void Stop() {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
            _are.Set();
        }
    }
}