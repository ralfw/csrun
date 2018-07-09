using System.Collections.Generic;

namespace csrun.data.domain
{
    internal class Sourcecode
    {
        public string Filename;
        public string[] Text;
        public LineMappings LineMappings = new LineMappings();
    }
}