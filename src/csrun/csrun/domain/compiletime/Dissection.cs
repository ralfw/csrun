using System.Collections.Generic;
using csrun.data.domain;

namespace csrun.domain.compiletime
{
    internal class Dissection
    {
        public static IEnumerable<Sourcecode> Dissect(Sourcecode csrunSource) {
            yield return csrunSource;
        }
    }
}