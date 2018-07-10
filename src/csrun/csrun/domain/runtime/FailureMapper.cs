using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using csrun.data.domain;

namespace csrun.domain.runtime
{
    internal static class FailureMapper
    {
        public static string MapRuntimeException(RuntimeError error, string[] csSourceText) {
            return $"{error}";
        }

        
        public static string[] MapCompiletimeErrors(IEnumerable<CompilerError> compilerErrors, string[] csSourceText) {
            return compilerErrors.Select(MapError).ToArray();

            string MapError(CompilerError error) {
                var (originFilename, originLineNumber) = MapLineNumber(error.LineNumber, csSourceText);
                return $"{originFilename}-{originLineNumber},{error.ColumnNumber}: {error.Description}";
            }
        }

        private static (string originFilename, int originLineNumber) MapLineNumber(int lineNumber, string[] csSourceText) {
            for(var i=lineNumber; i>=1; i--)
                if (OriginLabels.TryParse(csSourceText[i-1], out var originFilename, out var originStartLineNumber)) {
                    return (originFilename,
                            originStartLineNumber + (lineNumber - i) - 1);
                }
            throw new InvalidOperationException($"Missing origin label for line number {lineNumber}!");
        }
    }
}