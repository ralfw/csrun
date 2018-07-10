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
     * originLabel ::= "//#origin" <line number in original csrun source> "," <original csrun source filename> .
     *
     * The text following this label is from the file referenced at the line named and extends until
     * the label "//#endorigin".
     *
     * Example:
     *     ...
     *     //#origin 12,example.csrun
     *     ... // csrun code
     *     //#endorigin
     *     ...
     *
     * See also: FailureMapper{}, OriginLabels{}
     */
    internal static class Rendering
    {
        public static Sourcecode Render(Sourcecode[] csrunSections, string[] csTemplate)
        {
            return new Sourcecode {
                Section = Sourcecode.Sections.CSharp,
                Filename = Path.GetFileNameWithoutExtension(csrunSections.First().Filename) + ".cs",
                Text = csrunSections.Aggregate(csTemplate, Render_section)
            };
        }


        static string[] Render_section(string[] csTemplate, Sourcecode csrunSection) {
            var regionStartLineNumber = csTemplate.Select((line, index) => new {LineNumber = index + 1, Text = line})
                                              .First(line => line.Text.IndexOf(Regionname()) >= 0)
                                              .LineNumber;
            var csBeforeSection = csTemplate.Take(regionStartLineNumber);
            var csAfterSection = csTemplate.Skip(regionStartLineNumber);

            var originLabels = OriginLabels.Create(csrunSection.Filename, csrunSection.OriginLineNumber);
            return csBeforeSection.Concat(new[] {originLabels.startLabel})
                               .Concat(csrunSection.Text)
                               .Concat(new[] {originLabels.endLabel})
                               .Concat(csAfterSection)
                               .ToArray();


            string Regionname() {
                switch (csrunSection.Section) {
                    case Sourcecode.Sections.CSRunMain: return "#region main";
                    case Sourcecode.Sections.CSRunFunctions: return "#region functions";
                    case Sourcecode.Sections.CSRunTest: return "#region test";
                    default: throw new InvalidOperationException($"Cannot render section type {csrunSection.Section}!");
                }
            }
        }
    }
}