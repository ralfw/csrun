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
        [Test]
        public void Render_just_main()
        {
            var csrunMain = new Sourcecode {
                Section = Sourcecode.Sections.CSRunMain,
                Filename = "main.csrun",
                Text = new[] {
                    "var answer = 42;",
                    "Console.WriteLine(answer);"
                },
                OriginLineNumber = 1
            };

            var csTemplate = @"1
2
        #region main
        #endregion
5".Split('\n');

            var result = Rendering.Render(new[]{csrunMain}, csTemplate);
            var csSource = string.Join("\n", result.Text);
            
            Console.WriteLine($"{csSource}");
            
            Assert.AreEqual("main.cs", result.Filename);
            Assert.AreEqual(9, result.Text.Length);            
            Assert.IsTrue(csSource.IndexOf("//#origin 1,main.csrun") > 0);
            Assert.IsTrue(csSource.IndexOf("//#endorigin") > 0);
        }
    }
}