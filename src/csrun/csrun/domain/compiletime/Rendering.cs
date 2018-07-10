using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using csrun.data.domain;
using csrun.domain.runtime;

namespace csrun.domain.compiletime
{
    /*
     * Rendering creates csharp source code from csrun section code.
     *
     * csrun code is inserted into regions named like the secions, eg. #region functions.
     * There can be several #test sections in csrun code which all will be rendered into the same #region test, though.
     *
     * To later be able to map a csharp code line back to the original csrun code line labels are inserted
     * into the csharp code. Their syntax is:
     *
     * originLabel ::= "//#origin" <line number in original csrun source> "," <original csrun source filename>
     *
     * The text following this label is from the file referenced at the line named and extends until
     * the label "//#endorigin".
     *
     * See also: FailureMapper{}
     */
    internal static class Rendering
    {
        public static Sourcecode Render(Sourcecode[] csrunSections, string[] csTemplate)
        {
            var csText = Render_section(Sourcecode.Sections.CSRunMain, csrunSections, csTemplate);
            csText = Render_section(Sourcecode.Sections.CSRunFunctions, csrunSections, csText);
            return new Sourcecode {
                Section = Sourcecode.Sections.CSharp,
                Filename = Path.GetFileNameWithoutExtension(csrunSections.First().Filename) + ".cs",
                Text = csText
            };
        }


        static string[] Render_section(Sourcecode.Sections section, IEnumerable<Sourcecode> csrunSections, string[] csText) {
            var sectionSource = csrunSections.FirstOrDefault(s => s.Section == section);
            if (sectionSource == null) return csText;

            var regionStartLineNumber = csText.Select((line, index) => new {LineNumber = index + 1, Text = line})
                                              .First(line => line.Text.IndexOf(Regionname()) >= 0)
                                              .LineNumber;
            var csBeforeMain = csText.Take(regionStartLineNumber);
            var csAfterMain = csText.Skip(regionStartLineNumber);

            var originLabels = OriginLabels.Create(sectionSource.Filename, sectionSource.OriginLineNumber);
            return csBeforeMain.Concat(new[] {originLabels.startLabel})
                               .Concat(sectionSource.Text)
                               .Concat(new[] {originLabels.endLabel})
                               .Concat(csAfterMain)
                               .ToArray();


            string Regionname() {
                switch (section) {
                    case Sourcecode.Sections.CSRunMain: return "#region main";
                    case Sourcecode.Sections.CSRunFunctions: return "#region functions";
                    default: throw new InvalidOperationException();
                }
            }
        }
    }
}