using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using csrun.data.domain;

namespace csrun.domain.runtime
{
    internal class FailureMapper
    {
        private readonly string[] _csSource;

        public FailureMapper(string[] csSource) {
            _csSource = csSource;
        }

        
        public string MapRuntimeException(Exception exception) {
            return $"{exception}";
        }

        
        public string[] MapCompiletimeErrors(IEnumerable<CompilerError> compilerErrors) {
            return compilerErrors.Select(MapError).ToArray();

            string MapError(CompilerError error) {
                var (originFilename, originLineNumber) = MapLineNumber(error.LineNumber);
                return $"{originFilename}-{originLineNumber},{error.ColumnNumber}: {error.Description}";
            }
        }

        private (string originFilename, int originLineNumber) MapLineNumber(int lineNumber) {
            for(var i=lineNumber; i>=1; i--)
                if (OriginLabels.TryParse(_csSource[i-1], out var originFilename, out var originStartLineNumber)) {
                    return (originFilename,
                            originStartLineNumber + (lineNumber - i) - 1);
                }
            throw new InvalidOperationException($"Missing origin label for line number {lineNumber}!");
        }
    }
}