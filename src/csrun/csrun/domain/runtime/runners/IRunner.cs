using System.Collections.Generic;
using csrun.data.domain;

namespace csrun.domain.runtime.runners
{
    internal interface IRunner {
        IEnumerable<RuntimeResult> Run(Executable exe);
    }
}