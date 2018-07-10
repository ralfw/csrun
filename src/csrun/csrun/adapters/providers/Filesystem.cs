using System.IO;
using csrun.data.domain;

namespace csrun.adapters.providers
{
    internal class Filesystem
    {
        public Sourcecode ReadSource(string filename) {
            return new Sourcecode {
                Text = File.ReadAllLines(filename),
                Filename = filename
            };
        }

        
        public string[] LoadTemplate() {
            const string RENDER_TEMPLATE_FILENAME = "template.cs";
            var exePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var templateFilepath = Path.Combine(exePath, RENDER_TEMPLATE_FILENAME);

            return File.ReadAllLines(templateFilepath);
        }
    }
}