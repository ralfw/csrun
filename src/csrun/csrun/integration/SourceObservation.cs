using System;
using csrun.adapters.providers;

namespace csrun.integration
{
    internal class SourceObservation
    {
        private readonly Filesystem _fs;
        private readonly BlockingTimer _tim;
        
        public SourceObservation(Filesystem fs, BlockingTimer tim) {
            _fs = fs;
            _tim = tim;
        }

        
        public void WatchForChange(string filename, Action onChanged) {
            Console.WriteLine($"Started watching {filename}...");
            PollFileForChanges(filename, 
                () => {
                    onChanged();
                    Console.WriteLine($"\nAbort watch mode by pressing Ctrl-C");
                });
        }
        
        
        private void PollFileForChanges(string filename, Action onChanged) {
            var fileTimestamp = DateTime.MinValue;
            _tim.Start(() => {
                if (_fs.FileHasChanged(filename, ref fileTimestamp))
                    onChanged();
            });
            _tim.Wait();
        }
    }
}