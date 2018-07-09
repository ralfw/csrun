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
        public void Render()
        {
            var csrunMain = new Sourcecode {
                Filename = "main.csrun",
                Text = new[] {
                    "var answer = 42;",
                    "Console.WriteLine(answer);"
                }
            };

            
            var result = CSRenderer.Render(csrunMain);
            
            Debug.WriteLine($"{string.Join("\n", result.Text)}");
            
            Assert.AreEqual("main.cs", result.Filename);
            Assert.AreEqual(31, result.Text.Length);
            Assert.AreEqual(1, result.LineMappings.Mappings.Count());
            Assert.AreEqual(21, result.LineMappings.Mappings.First().DestinationFromLineNumber);
            Assert.AreEqual(22, result.LineMappings.Mappings.First().DestinationToLineNumber);
        }
    }
}