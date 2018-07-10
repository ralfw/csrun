﻿using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using csrun.data.domain;

namespace csrun.domain.compiletime
{
    /*
     * csrun code is made up of different sections:
     *     - the main section is code to be executed in run-mode. it's not prefixed and is defined at the beginning
     *       of the csrun source.
     *     - the functions section is code containing function definitions. these functions get called from other
     *       functions or main code or test code. it's prefixed by #functions.
     *     - each test needs to be prefixed by #test followed by a label. tests are run in test- and watch-mode.
     *
     * there may only be one main section and one functions section in the csrun code.
     */
    internal static class Dissection
    {
        public static IEnumerable<Sourcecode> Dissect(Sourcecode csrunSource)
        {
            var mainSource = Extract_main_section(csrunSource.Filename, csrunSource.Text);
            var functionsSource = Extract_functions_section(csrunSource.Filename, csrunSource.Text);
            var testSources = Extract_test_sections(csrunSource.Filename, csrunSource.Text);

            if (IsNotEmpty(mainSource)) yield return mainSource;
            if (IsNotEmpty(functionsSource)) yield return functionsSource;
            foreach (var ts in testSources) yield return ts;


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
        
        
        static IEnumerable<Sourcecode> Extract_test_sections(string filename, string[] text) {
            var sections = new List<Sourcecode>();
            var header = "";
            var body = new List<string>();
            var fromLineNumber = 0;
            var currLineNumber = 0;
            
            var inTest = false;
            foreach (var l in text) {
                currLineNumber++;
                if (l.Trim().StartsWith("#test")) {
                    BuildSection();
                    header = l.Trim();
                    fromLineNumber = currLineNumber + 1;
                    inTest = true;
                }
                else if (l.Trim().StartsWith("#functions")) {
                    BuildSection();
                    inTest = false;
                }
                else if (inTest)
                    body.Add(l);
            }
            BuildSection();
            return sections;


            void BuildSection() {
                if (body.Count == 0) return;

                var testLabel = header.Substring("#test".Length).Trim();
                
                var csrunSource = new Sourcecode {
                    Section = Sourcecode.Sections.CSRunTest,
                    Filename = $"{filename}#{testLabel}",
                    OriginLineNumber = fromLineNumber,
                    Text = body.ToArray()
                };
                body.Clear();
                
                sections.Add(csrunSource);
            }
        }
    }
}