using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using csrun.data.domain;

namespace csrun.domain.runtime
{
    internal static class FailureMapper
    {
        public static string MapRuntimeException(Exception ex, string[] csSourceText) {
            return $"{ex}";
        }
        
        
        public static string MapTestException(Exception ex, string[] csSourceText) {
            return $"{ex.InnerException.Message}";
        }

        
        public static string[] MapCompiletimeErrors(IEnumerable<CompilerError> compilerErrors, string[] csSourceText) {
            return compilerErrors.Select(MapError).ToArray();

            string MapError(CompilerError error) {
                try {
                    var (originFilename, originLineNumber) = MapLineNumber(error.LineNumber, csSourceText);
                    return $"{originFilename}-{originLineNumber},{error.ColumnNumber}: {error.Description}";
                }
                catch (Exception ex) {
                    throw new InvalidOperationException($"Cannot map C# compiler error <{error.Description}> @ ({error.LineNumber},{error.ColumnNumber}) to .csrun source!", ex);
                }
            }
        }

        private static (string originFilename, int originLineNumber) MapLineNumber(int lineNumber, string[] csSourceText) {
            for(var i=lineNumber; i>=1; i--)
                if (OriginLabels.TryParse(csSourceText[i-1], out var originFilename, out var originStartLineNumber)) {
                    return (originFilename,
                            originStartLineNumber + (lineNumber - i) - 1);
                }
            throw new InvalidOperationException($"Missing //#origin label for line number {lineNumber}!");
        }
    }
}