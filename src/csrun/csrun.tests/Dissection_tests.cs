using System.Collections.Generic;
using System.Linq;
using csrun.data.domain;
using csrun.domain.compiletime;
using NUnit.Framework;

namespace csrun.tests
{
    [TestFixture]
    public class Dissection_tests
    {
        [Test]
        public void Main_and_functions()
        {
            var csrunSource = new Sourcecode
            {
                Filename = "main.csrun",
                Text = new[] {
                    "main 1",
                    "main 2",
                    "#functions",
                    "f 1",
                    "f 2",
                    "f 3"
                }
            };

            var result = Dissection.Dissect(csrunSource).ToArray();

            Assert.AreEqual(2, result.Length);
            
            Assert.AreEqual(Sourcecode.Sections.CSRunMain, result[0].Section);
            Assert.AreEqual(new[]{"main 1", "main 2"}, result[0].Text);
            Assert.AreEqual(1, result[0].OriginLineNumber);
            
            Assert.AreEqual(Sourcecode.Sections.CSRunFunctions, result[1].Section);
            Assert.AreEqual(new[]{"f 1", "f 2", "f 3"}, result[1].Text);
            Assert.AreEqual(4, result[1].OriginLineNumber);
        }
        
        
        [Test]
        public void Main_without_functions()
        {
            var csrunSource = new Sourcecode
            {
                Filename = "main.csrun",
                Text = new[] {
                    "main 1",
                    "main 2",
                }
            };

            var result = Dissection.Dissect(csrunSource).ToArray();

            Assert.AreEqual(1, result.Length);
            
            Assert.AreEqual(Sourcecode.Sections.CSRunMain, result[0].Section);
            Assert.AreEqual(new[]{"main 1", "main 2"}, result[0].Text);
            Assert.AreEqual(1, result[0].OriginLineNumber);
        }
        
        
        [Test]
        public void Functions_without_main()
        {
            var csrunSource = new Sourcecode
            {
                Filename = "main.csrun",
                Text = new[] {
                    "#functions",
                    "f 1",
                    "f 2",
                    "f 3"
                }
            };

            var result = Dissection.Dissect(csrunSource).ToArray();

            Assert.AreEqual(1, result.Length);
            
            Assert.AreEqual(Sourcecode.Sections.CSRunFunctions, result[0].Section);
            Assert.AreEqual(new[]{"f 1", "f 2", "f 3"}, result[0].Text);
            Assert.AreEqual(2, result[0].OriginLineNumber);
        }
    }
}