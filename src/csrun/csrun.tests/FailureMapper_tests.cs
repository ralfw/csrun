using System;
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
        public void MapCompilerError()
        {
            var csSource = @"1
2
//#origin 1,origin.csrun
4->1
5->2
//#endorigin
7".Split('\n');
            var sut = new FailureMapper(csSource);
            
            var result = sut.MapCompiletimeErrors(new[] {new CompilerError {
                Filename = "destination.cs",
                LineNumber = 5,
                ColumnNumber = 10,
                Description = "desc"
            }});

            Assert.IsTrue(result.First().StartsWith($"origin.csrun-2,"));
        }
        
        
        [Test]
        public void MapCompilerError_after_origin()
        {
            var csSource = @"1
2
//#origin 1,origin.csrun
4->1
5->2
//#endorigin
7".Split('\n');
            var sut = new FailureMapper(csSource);
            
            var result = sut.MapCompiletimeErrors(new[] {new CompilerError {
                Filename = "destination.cs",
                LineNumber = 6,
                ColumnNumber = 0,
                Description = "desc"
            }});

            Console.WriteLine(result.First());
            
            Assert.IsTrue(result.First().StartsWith($"origin.csrun-3,"));
        }
    }
}