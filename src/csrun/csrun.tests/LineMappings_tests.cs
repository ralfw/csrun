using csrun.data.domain;
using NUnit.Framework;

namespace csrun.tests
{
    [TestFixture]
    public class LineMappings_tests
    {
        private LineMappings _sut;

        public LineMappings_tests()
        {
            _sut = new LineMappings();
            _sut.Add(new LineMapping
            {
                DesinationFilename = "destination.cs",
                DestinationFromLineNumber = 6,
                DestinationToLineNumber = 7,
                OriginFilename = "origin1.csrun",
                OriginFromLineNumber = 1
            });
            _sut.Add(new LineMapping
            {
                DesinationFilename = "destination.cs",
                DestinationFromLineNumber = 8,
                DestinationToLineNumber = 8,
                OriginFilename = "origin2.csrun",
                OriginFromLineNumber = 3
            });
            _sut.Add(new LineMapping
            {
                DesinationFilename = "destination.cs",
                DestinationFromLineNumber = 10,
                DestinationToLineNumber = 13,
                OriginFilename = "origin3.csrun",
                OriginFromLineNumber = 7
            });
        }
        
        
        [TestCase(6, "origin1.csrun", 1)]
        [TestCase(7, "origin1.csrun", 2)]
        [TestCase(8, "origin2.csrun", 3)]
        [TestCase(12, "origin3.csrun", 9)]
        public void In_range(int destinationLineNumber, string expectedFilename, int expectedLineNumber)
        {
            var result = _sut.MapToOrigin("destination.cs", destinationLineNumber);
            
            Assert.AreEqual(expectedFilename, result.originFilename);
            Assert.AreEqual(expectedLineNumber, result.originLineNumber);
        }
    }
}