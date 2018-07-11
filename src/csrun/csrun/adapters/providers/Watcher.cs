using System;
using System.IO;
using System.Threading;

namespace csrun.adapters.providers
{
    internal class Watcher
    {
        private const int DUETIME_MS = 0;
        private const int INTERVAL_MS = 1000;

        private readonly Filesystem _fs;
        private readonly string _filename;
        private Timer _timer;
        private AutoResetEvent _are;
        
        public Watcher(Filesystem fs, string filename) {
            _fs = fs;
            _filename = filename;
        }

        
        public void Start(Action onChanged) {
            Console.WriteLine($"Started watching {_filename}...");
            
            PollFileForChanges(() => {
                onChanged();
                Console.WriteLine($"\nAbort watch mode by pressing Ctrl-C");
            });
        }
        
        private void PollFileForChanges(Action onChanged) {
            DateTime fileTimestamp = DateTime.MinValue;
            var busy = false;
            _timer = new Timer(_ => {
                if (busy) return;
                if (_fs.FileHasChanged(_filename, ref fileTimestamp)) {
                    busy = true;
                    onChanged();
                    busy = false;
                }
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