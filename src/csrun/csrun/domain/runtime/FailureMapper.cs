using System;
using System.Collections.Generic;
using System.Linq;
using csrun.data.domain;

namespace csrun.domain.runtime
{
    internal class FailureMapper
    {
        private readonly LineMappings _mappings;

        public FailureMapper(LineMappings mappings) {
            _mappings = mappings;
        }

        
        public string MapRuntimeException(Exception exception) {
            return $"{exception}";
        }

        
        public string[] MapCompiletimeErrors(IEnumerable<CompilerError> compilerErrors) {
            return compilerErrors.Select(MapError).ToArray();

            string MapError(CompilerError error) {
                var (originFilename, originLineNumber) = _mappings.MapToOrigin(error.Filename, error.LineNumber);
                return $"{originFilename}-{originLineNumber},{error.ColumnNumber}: {error.Description}";
            }
        }
    }
}