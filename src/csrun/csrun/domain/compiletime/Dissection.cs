using System.Collections.Generic;
using csrun.data.domain;

namespace csrun.domain.compiletime
{
    internal class Dissection
    {
        public static IEnumerable<Sourcecode> Dissect(Sourcecode csrunSource)
        {
            yield return new Sourcecode {
                Section = Sourcecode.Sections.CSRunMain,
                Filename = csrunSource.Filename,
                Text = csrunSource.Text,
                LineMappings = csrunSource.LineMappings
            };
        }
    }
}