using System;
using System.Linq;
using csrun.data.domain;
using csrun.domain.runtime;
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
            
            var result = FailureMapper.MapCompiletimeErrors(new[] {new CompilerError {
                Filename = "destination.cs",
                LineNumber = 5,
                ColumnNumber = 10,
                Description = "desc"
            }}, csSource);

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
            
            var result = FailureMapper.MapCompiletimeErrors(new[] {new CompilerError {
                Filename = "destination.cs",
                LineNumber = 6,
                ColumnNumber = 0,
                Description = "desc"
            }}, csSource);

            Console.WriteLine(result.First());
            
            Assert.IsTrue(result.First().StartsWith($"origin.csrun-3,"));
        }
        
        
        [Test]
        public void MapCompilerError_before_origin()
        {
            var csSource = @"1
2
//#origin 1,origin.csrun
4->1
5->2
//#endorigin
7".Split('\n');

            try
            {
                var result = FailureMapper.MapCompiletimeErrors(new[]
                {
                    new CompilerError
                    {
                        Filename = "destination.cs",
                        LineNumber = 1,
                        ColumnNumber = 0,
                        Description = "desc"
                    }
                }, csSource);

                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.IndexOf("Cannot map") >= 0);
            }
        }
    }
}