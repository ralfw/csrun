using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using csrun.data.domain;

namespace csrun.domain.compiletime
{
    internal static class Rendering
    {
        public static Sourcecode Render(IEnumerable<Sourcecode> csrunSources, string[] csTemplate) {
            var regionMainLineNumber = csTemplate.Select((line, index) => new {LineNumber = index + 1, Text = line})
                                                 .First(line => line.Text.IndexOf("#region main") >= 0)
                                                 .LineNumber;
            var csBeforeMain = csTemplate.Take(regionMainLineNumber);
            var csAfterMain = csTemplate.Skip(regionMainLineNumber);

            var merged = csBeforeMain.Concat(csrunSources.First().Text).Concat(csAfterMain);

            var csSource = new Sourcecode {
                Filename = Path.GetFileNameWithoutExtension(csrunSources.First().Filename) + ".cs",
                Text = merged.ToArray()
            };
            csSource.LineMappings.Add(new LineMapping {
                DesinationFilename = csSource.Filename,
                DestinationFromLineNumber = regionMainLineNumber + 1,
                DestinationToLineNumber = regionMainLineNumber + csrunSources.First().Text.Length,
                OriginFilename = csrunSources.First().Filename,
                OriginFromLineNumber = 1
            });
            return csSource;
        }
    }
}