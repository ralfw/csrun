using System;
using System.IO;
using System.Linq;
using csrun.data.domain;

namespace csrun.domain.compiletime
{
    internal static class CSRenderer
    {
        public static Sourcecode Render(Sourcecode csrunSource) {
            var csTemplate = Load_template();
            return Render(csrunSource, csTemplate);
        }


        /*
         * Loading the template is co-located with rendering because it's closely related.
         * The template might get hard coded or moved to a resource. Setting up a provider
         * to load it right seems to be overengineerig.
         */
        private static Sourcecode Load_template()
        {
            const string RENDER_TEMPLATE_FILENAME = "template.cs";
            var exePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var templateFilepath = Path.Combine(exePath, RENDER_TEMPLATE_FILENAME);

            var csTemplate = new Sourcecode
            {
                Filename = RENDER_TEMPLATE_FILENAME,
                Text = File.ReadAllLines(templateFilepath)
            };
            return csTemplate;
        }
        
        
        private static Sourcecode Render(Sourcecode csrunSource, Sourcecode csTemplate)
        {
            var regionMainLineNumber = csTemplate.Text.Select((line, index) => new {LineNumber = index + 1, Text = line})
                .First(line => line.Text.IndexOf("#region main") >= 0)
                .LineNumber;
            var csBeforeMain = csTemplate.Text.Take(regionMainLineNumber);
            var csAfterMain = csTemplate.Text.Skip(regionMainLineNumber);

            var merged = csBeforeMain.Concat(csrunSource.Text).Concat(csAfterMain);

            var csSource = new Sourcecode {
                Filename = Path.GetFileNameWithoutExtension(csrunSource.Filename) + ".cs",
                Text = merged.ToArray()
            };
            csSource.LineMappings.Add(new LineMapping {
                DesinationFilename = csSource.Filename,
                DestinationFromLineNumber = regionMainLineNumber + 1,
                DestinationToLineNumber = regionMainLineNumber + csrunSource.Text.Length,
                OriginFilename = csrunSource.Filename,
                OriginFromLineNumber = 1
            });
            return csSource;
        }
    }
}