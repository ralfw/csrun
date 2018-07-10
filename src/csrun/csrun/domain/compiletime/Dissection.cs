using System.Collections.Generic;
using System.Linq;
using csrun.data.domain;

namespace csrun.domain.compiletime
{
    internal static class Dissection
    {
        public static IEnumerable<Sourcecode> Dissect(Sourcecode csrunSource)
        {
            var mainSource = Extract_main_section(csrunSource.Filename, csrunSource.Text);
            var functionsSource = Extract_functions_section(csrunSource.Filename, csrunSource.Text);

            if (IsNotEmpty(mainSource)) yield return mainSource;
            if (IsNotEmpty(functionsSource)) yield return functionsSource;


            bool IsNotEmpty(Sourcecode source) => source.Text.Length > 0;
        }


        static Sourcecode Extract_main_section(string filename, string[] text) {
            var (mainText, fromLineNumber, toLineNumber) = Extract_main_source_text(text);
            
            var mainSource = new Sourcecode {
                Section = Sourcecode.Sections.CSRunMain,
                Filename = filename,
                Text = mainText,
            };
            mainSource.LineMappings.Add(new LineMapping {
                DesinationFilename = mainSource.Filename,
                DestinationFromLineNumber = fromLineNumber,
                DestinationToLineNumber = toLineNumber,
                OriginFilename = filename,
                OriginFromLineNumber = fromLineNumber
            });
            return mainSource;
        }

        static (string[] text, int fromLineNumber, int toLineNumber) Extract_main_source_text(string[] text) {
            var lines = new List<string>();
            foreach (var l in text) {
                if (l.Trim().StartsWith("#functions") || l.Trim().StartsWith("#test"))
                    break;
                lines.Add(l);
            }
            return (lines.ToArray(), 1, lines.Count);
        }


        static Sourcecode Extract_functions_section(string filename, string[] text) {
            var (functionsText, fromLineNumber, toLineNumber) = Extract_functions_source_text(text);
            
            var functionsSource = new Sourcecode {
                Section = Sourcecode.Sections.CSRunFunctions,
                Filename = filename,
                Text = functionsText,
            };
            functionsSource.LineMappings.Add(new LineMapping {
                DesinationFilename = functionsSource.Filename,
                DestinationFromLineNumber = fromLineNumber,
                DestinationToLineNumber = toLineNumber,
                OriginFilename = filename,
                OriginFromLineNumber = fromLineNumber
            });
            return functionsSource;
        }

        static (string[] text, int fromLineNumber, int toLineNumber) Extract_functions_source_text(string[] text) {
            var lines = new List<string>();
            var fromLineNumber = 0;
            var currLineNumber = 0;
            
            var inFunctions = false;
            foreach (var l in text) {
                currLineNumber++;
                if (l.Trim().StartsWith("#functions")) {
                    inFunctions = true;
                    fromLineNumber = currLineNumber + 1;
                }
                else if (l.Trim().StartsWith("#test"))
                    inFunctions = false;
                else if (inFunctions)
                    lines.Add(l);
            }
            
            var toLineNumber = fromLineNumber + lines.Count - 1;

            return (lines.ToArray(), fromLineNumber, toLineNumber);
        }
    }
}