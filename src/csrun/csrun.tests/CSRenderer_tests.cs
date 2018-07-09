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
                Filename = "main.csrun",
                Text = new[] {
                    "var answer = 42;",
                    "Console.WriteLine(answer);"
                }
            };

            var csTemplate = @"using System;
public class Program
{
    public static void Main(string[] args)
    {
        var prog = new Program();
        prog.Run(args);
    }

    private void Run(string[] args)
    {
        #region main
        #endregion
    }
  
    #region functions
    #endregion
    
    #region tests
    #endregion
}".Split(new[]{'\n'}, StringSplitOptions.RemoveEmptyEntries);

            var result = Rendering.Render(csrunMain, csTemplate);
            
            Debug.WriteLine($"{string.Join("\n", result.Text)}");
            
            Assert.AreEqual("main.cs", result.Filename);
            Assert.AreEqual(22, result.Text.Length);
            Assert.AreEqual(1, result.LineMappings.Mappings.Count());
            Assert.AreEqual(12, result.LineMappings.Mappings.First().DestinationFromLineNumber);
            Assert.AreEqual(13, result.LineMappings.Mappings.First().DestinationToLineNumber);
        }
    }
}