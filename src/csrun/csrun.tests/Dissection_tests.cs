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
                    "f 3",
                    "#functions",
                    "f 4"
                }
            };

            var result = Dissection.Dissect(csrunSource).ToArray();

            Assert.AreEqual(3, result.Length);
            
            Assert.AreEqual(Sourcecode.Sections.CSRunMain, result[0].Section);
            Assert.AreEqual(new[]{"main 1", "main 2"}, result[0].Text);
            Assert.AreEqual(1, result[0].OriginLineNumber);
            
            Assert.AreEqual(Sourcecode.Sections.CSRunFunctions, result[1].Section);
            Assert.AreEqual(new[]{"f 1", "f 2", "f 3"}, result[1].Text);
            Assert.AreEqual(4, result[1].OriginLineNumber);
            
            Assert.AreEqual(Sourcecode.Sections.CSRunFunctions, result[2].Section);
            Assert.AreEqual(new[]{"f 4"}, result[2].Text);
            Assert.AreEqual(8, result[2].OriginLineNumber);
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
        
        
        [Test]
        public void Main_tests_and_functions()
        {
            var csrunSource = new Sourcecode
            {
                Filename = "main.csrun",
                Text = @"m1
m2
#functions
f1
f2
#test test 1
t1.1
t1.2
#test test 2
t2.1
t2.2
#test
t3.1
t3.2
t3.3
#functions
f3
f4".Split('\n')
            };

            var result = Dissection.Dissect(csrunSource).ToArray();

            
            Assert.AreEqual(Sourcecode.Sections.CSRunMain, result[0].Section);
            Assert.AreEqual(new[]{"m1","m2"}, result[0].Text);
            Assert.AreEqual(1, result[0].OriginLineNumber);

            Assert.AreEqual(Sourcecode.Sections.CSRunFunctions, result[1].Section);
            Assert.AreEqual(new[]{"f1","f2"}, result[1].Text);
            Assert.AreEqual(4, result[1].OriginLineNumber);
            
            Assert.AreEqual(Sourcecode.Sections.CSRunTest, result[2].Section);
            Assert.AreEqual("main.csrun", result[2].Filename);
            Assert.AreEqual("test 1", result[2].Label);
            Assert.AreEqual(new[]{"t1.1","t1.2"}, result[2].Text);
            Assert.AreEqual(7, result[2].OriginLineNumber);
            
            Assert.AreEqual(Sourcecode.Sections.CSRunTest, result[3].Section);
            Assert.AreEqual("main.csrun", result[3].Filename);
            Assert.AreEqual("test 2", result[3].Label);
            Assert.AreEqual(new[]{"t2.1","t2.2"}, result[3].Text);
            Assert.AreEqual(10, result[3].OriginLineNumber);
            
            Assert.AreEqual(Sourcecode.Sections.CSRunTest, result[4].Section);
            Assert.AreEqual("main.csrun", result[4].Filename);
            Assert.AreEqual("", result[4].Label);
            Assert.AreEqual(new[]{"t3.1","t3.2","t3.3"}, result[4].Text);
            Assert.AreEqual(13, result[4].OriginLineNumber);
            
            Assert.AreEqual(Sourcecode.Sections.CSRunFunctions, result[5].Section);
            Assert.AreEqual(new[]{"f3","f4"}, result[5].Text);
            Assert.AreEqual(17, result[5].OriginLineNumber);
            
            Assert.AreEqual(6, result.Length);
        }
        
    }
}