using System.Collections.Generic;
using System.Linq;
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
            var sections = new List<Sourcecode>();
            var currentSectionText = new List<string>();
            var currentSection = OpenSection(Sourcecode.Sections.CSRunMain, 1);
            var lineNumber = 0;
            foreach (var line in csrunSource.Text) {
                lineNumber++;
                switch (Line_classification(line)) {
                    case Sourcecode.Sections.CSRunFunctions:
                        FlushSection();
                        currentSection = OpenSection(Sourcecode.Sections.CSRunFunctions, lineNumber + 1);
                        break;
                    case Sourcecode.Sections.CSRunTest:
                        FlushSection();
                        currentSection = OpenSection(Sourcecode.Sections.CSRunTest, lineNumber + 1);
                        currentSection.Label = line.Trim().Substring("#test".Length).Trim();
                        break;
                    default:
                        currentSectionText.Add(line);
                        break;
                }
            }
            FlushSection();
            return sections;


            Sourcecode.Sections Line_classification(string text) {
                if (text.Trim().StartsWith("#functions"))
                    return Sourcecode.Sections.CSRunFunctions;
                if (text.Trim().StartsWith("#test"))
                    return Sourcecode.Sections.CSRunTest;
                return Sourcecode.Sections.CSRunRaw;
            }

            Sourcecode OpenSection(Sourcecode.Sections sectionType, int originLineNumber) {
                return new Sourcecode {
                    Section = sectionType,
                    Filename = csrunSource.Filename,
                    OriginLineNumber = originLineNumber
                };
            }

            void FlushSection() {
                if (!currentSectionText.Any()) return;
                currentSection.Text = currentSectionText.ToArray();
                sections.Add(currentSection);
                
                currentSectionText.Clear();
                currentSection = null;
            }
        }
    }
}