﻿using System.Collections.Generic;
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
            var (mainText, fromLineNumber) = Extract_main_source_text(text);
            
            return new Sourcecode {
                Section = Sourcecode.Sections.CSRunMain,
                Filename = filename,
                OriginLineNumber = fromLineNumber,
                Text = mainText,
            };
        }

        static (string[] text, int fromLineNumber) Extract_main_source_text(string[] text) {
            return (text.TakeWhile(l => !l.Trim().StartsWith("#functions") && !l.Trim().StartsWith("#test")).ToArray(), 
                    1);
        }


        static Sourcecode Extract_functions_section(string filename, string[] text) {
            var (functionsText, fromLineNumber) = Extract_functions_source_text(text);
            
            return new Sourcecode {
                Section = Sourcecode.Sections.CSRunFunctions,
                Filename = filename,
                OriginLineNumber = fromLineNumber,
                Text = functionsText
            };
        }

        static (string[] text, int fromLineNumber) Extract_functions_source_text(string[] text) {
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
                    break;
                else if (inFunctions)
                    lines.Add(l);
            }
            
            return (lines.ToArray(), fromLineNumber);
        }
    }
}