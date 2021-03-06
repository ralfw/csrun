﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace csrun.domain.runtime
{
    internal class Executable
    {
        private readonly Assembly _assm;

        public Executable(Assembly assm) { _assm = assm; }

        
        public void Main() {
            var tProg = _assm.GetType("Program");
            var mMain = tProg.GetMethod("Main");
            mMain.Invoke(null, new object[]{new string[0]});
        }


        public IEnumerable<(string name, string label)> Testmethods {
            get {
                var tProg = _assm.GetType("Program");
                var methods = tProg.GetMethods();
                foreach (var m in methods) {
                    var testAttr = m.GetCustomAttribute<NUnit.Framework.TestAttribute>();
                    if (testAttr != null) {
                        yield return (m.Name, testAttr.Description ?? "[no name]");
                    }
                }
            }
        }

        
        public void Test(string testmethodnames) {
            var tProg = _assm.GetType("Program");
            var mTest = tProg.GetMethod(testmethodnames);
            var oProg = Activator.CreateInstance(tProg);
            mTest.Invoke(oProg, new object[0]);
        }
    }
}