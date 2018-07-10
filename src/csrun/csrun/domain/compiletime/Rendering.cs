using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using csrun.data.domain;

namespace csrun.domain.compiletime
{
    internal static class Rendering
    {
        public static Sourcecode Render(Sourcecode[] csrunSections, string[] csTemplate)
        {
            var mainRendered = Render_section(Sourcecode.Sections.CSRunMain, csrunSections, csTemplate);
            var functionsRendered = Render_section(Sourcecode.Sections.CSRunFunctions, csrunSections, mainRendered.text);
            var lineMappings = Collect_mappings(mainRendered.lineMapping, functionsRendered.lineMapping);
            
            var csSource = new Sourcecode {
                Section = Sourcecode.Sections.CSharp,
                Filename = lineMappings.Mappings.First().DesinationFilename,
                Text = functionsRendered.text,
                LineMappings = lineMappings
            };

            return csSource;

            LineMappings Collect_mappings(LineMapping mainMapping, LineMapping functionsMapping) {
                var mappings = new LineMappings();
                if (mainMapping != null) mappings.Add(mainMapping);
                if (functionsMapping != null) mappings.Add(functionsMapping);
                return mappings;
            }
        }


        static (string[] text, LineMapping lineMapping) Render_section(Sourcecode.Sections section, 
                                                                       IEnumerable<Sourcecode> csrunSections, 
                                                                       string[] csText) {
            var sectionSource = csrunSections.FirstOrDefault(s => s.Section == section);
            if (sectionSource == null) return (csText, null);

            var regionStartLineNumber = csText.Select((line, index) => new {LineNumber = index + 1, Text = line})
                                              .First(line => line.Text.IndexOf(Regionname()) >= 0)
                                              .LineNumber;
            var csBeforeMain = csText.Take(regionStartLineNumber);
            var csAfterMain = csText.Skip(regionStartLineNumber);
            
            var mergedText = csBeforeMain.Concat(sectionSource.Text).Concat(csAfterMain);

            var lineMapping = new LineMapping {
                DesinationFilename = Path.GetFileNameWithoutExtension(sectionSource.Filename) + ".cs",
                DestinationFromLineNumber = regionStartLineNumber + 1,
                DestinationToLineNumber = regionStartLineNumber + sectionSource.Text.Length,
                OriginFilename = sectionSource.Filename,
                OriginFromLineNumber = sectionSource.LineMappings.Mappings.First().OriginFromLineNumber
            };
            return (mergedText.ToArray(), lineMapping);


            string Regionname() {
                switch (section) {
                    case Sourcecode.Sections.CSRunMain: return "#region main";
                    case Sourcecode.Sections.CSRunFunctions: return "#region functions";
                }
            }
        }
    }
}