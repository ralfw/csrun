using System.Collections.Generic;

namespace csrun.data.domain
{
    internal class Sourcecode
    {
        public enum Sections {
            CSRunRaw,
            CSRunMain,
            CSRunFunctions,
            CSRunTest,
            CSharp
        }

        public Sections Section;
        public string Filename;
        public string[] Text;
        public LineMappings LineMappings = new LineMappings();
    }
}