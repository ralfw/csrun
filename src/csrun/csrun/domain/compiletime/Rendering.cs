using System;
using System.IO;
using System.Linq;
using csrun.data.domain;

namespace csrun.domain.compiletime
{
    internal static class Rendering
    {
        public static Sourcecode Render(Sourcecode csrunSource, string[] csTemplate) {
            var regionMainLineNumber = csTemplate.Select((line, index) => new {LineNumber = index + 1, Text = line})
                                                 .First(line => line.Text.IndexOf("#region main") >= 0)
                                                 .LineNumber;
            var csBeforeMain = csTemplate.Take(regionMainLineNumber);
            var csAfterMain = csTemplate.Skip(regionMainLineNumber);

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