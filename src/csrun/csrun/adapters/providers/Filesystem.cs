using System.IO;
using csrun.data.domain;

namespace csrun
{
    internal class Filesystem
    {
        public Sourcecode ReadSource(string filename) {
            return new Sourcecode {
                Text = File.ReadAllLines(filename),
                Filename = filename
            };
        }
    }
}