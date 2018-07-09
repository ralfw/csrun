using System.Collections.Generic;
using System.Linq;

namespace csrun.data.domain
{
    internal class LineMappings
    {
        private readonly List<LineMapping> _mappings;

        public LineMappings() {
            _mappings = new List<LineMapping>();
        }

        public void Add(LineMapping mapping) => _mappings.Add(mapping);

        public IEnumerable<LineMapping> Mappings => _mappings;

        public (string originFilename, int originLineNumber) MapToOrigin(string destinationFilename, int destinationLineNumber)
        {
            var mapping = _mappings.Where(m => m.DesinationFilename == destinationFilename)
                                   .First(m => m.DestinationFromLineNumber <= destinationLineNumber &&
                                               destinationLineNumber <= m.DestinationToLineNumber);
            return (mapping.OriginFilename, mapping.MapToOrigin(destinationLineNumber));
        }
    }

    
    internal class LineMapping {
        public string DesinationFilename;
        public int DestinationFromLineNumber;
        public int DestinationToLineNumber;
        public string OriginFilename;
        public int OriginFromLineNumber;

        public int MapToOrigin(int destinationLineNumber)
            => OriginFromLineNumber + (destinationLineNumber - DestinationFromLineNumber);
    }
}