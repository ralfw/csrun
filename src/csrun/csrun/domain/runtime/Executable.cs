using System;
using System.Reflection;

namespace csrun.domain.runtime
{
    internal class Executable
    {
        private readonly Assembly _assm;

        public Executable(Assembly assm) {
            _assm = assm;
        }

        public void Main() {
            var tProg = _assm.GetType("Program");
            var mMain = tProg.GetMethod("Main");
            mMain.Invoke(null, new object[]{new string[0]});
        }
    }
}