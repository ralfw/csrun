using System.Linq;
using csrun.data.domain;
using csrun.domain.runtime;
using csrun.data.domain;
using NUnit.Framework;

namespace csrun.tests
{
    [TestFixture]
    public class FailureMapper_tests
    {
        [Test]
        public void MapCompilerErrors()
        {
            var mappings = new LineMappings();
            mappings.Add(new LineMapping
            {
                DesinationFilename = "destination.cs",
                DestinationFromLineNumber = 6,
                DestinationToLineNumber = 7,
                OriginFilename = "origin.csrun",
                OriginFromLineNumber = 1
            });
            var sut = new FailureMapper(mappings);
            
            var result = sut.MapCompiletimeErrors(new[] {new CompilerError {
                Filename = "destination.cs",
                LineNumber = 6,
                ColumnNumber = 10,
                Description = "desc"
            }});

            Assert.IsTrue(result.First().StartsWith($"origin.csrun-1,"));
        }
    }
}