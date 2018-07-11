using System;
using System.Diagnostics;
using System.Linq;
using csrun.data.domain;
using csrun.domain.compiletime;
using NUnit.Framework;

namespace csrun.tests
{
    [TestFixture]
    public class CSRenderer_tests
    {
        // Watch the console output of the test!
        [Test]
        public void Render_main_with_functions_and_tests()
        {
            var sections = new[]
            {
                new Sourcecode {
                    Section = Sourcecode.Sections.CSRunMain,
                    Filename = "main.csrun",
                    Text = new[] {
                        "m1",
                        "m2"
                    },
                    OriginLineNumber = 1
                },
                new Sourcecode {
                    Section = Sourcecode.Sections.CSRunFunctions,
                    Filename = "main.csrun",
                    Text = new[] {
                        "f1.1",
                        "f1.2"
                    },
                    OriginLineNumber = 10
                },
                new Sourcecode {
                    Section = Sourcecode.Sections.CSRunTest,
                    Filename = "main.csrun",
                    Label = "Test 1",
                    Text = new[] {
                        "t1.1",
                        "t1.2"
                    },
                    OriginLineNumber = 100
                },
                new Sourcecode {
                    Section = Sourcecode.Sections.CSRunTest,
                    Filename = "main.csrun",
                    Label = "Test 2",
                    Text = new[] {
                        "t2.1",
                        "t2.2"
                    },
                    OriginLineNumber = 200
                },
                new Sourcecode {
                    Section = Sourcecode.Sections.CSRunFunctions,
                    Filename = "main.csrun",
                    Text = new[] {
                        "f2.1",
                        "f2.2"
                    },
                    OriginLineNumber = 20
                },
                new Sourcecode {
                    Section = Sourcecode.Sections.CSRunTest,
                    Filename = "main.csrun",
                    Label = "",
                    Text = new[] {
                        "t3.1",
                        "t3.2",
                        "t3.3"
                    },
                    OriginLineNumber = 300
                }
            };

            var csTemplate = @"1
2
        #region main
        #endregion
5
        #region functions
        #endregion
6
        #region test
        #endregion
11".Split('\n');

            var result = Rendering.Render(sections, csTemplate);
            var csSource = string.Join("\n", result.Text);
            
            Console.WriteLine($"{csSource}");
            
            Assert.AreEqual("main.cs", result.Filename);
            Assert.AreEqual(42, result.Text.Length);            
            Assert.IsTrue(csSource.IndexOf("//#origin 1,main.csrun") > 0);
            Assert.IsTrue(csSource.IndexOf("//#endorigin") > 0);
            Assert.IsTrue(csSource.IndexOf("//#origin 20,main.csrun") > 0);
            Assert.IsTrue(csSource.IndexOf("//#origin 300,main.csrun") > 0);
        }
    }
}