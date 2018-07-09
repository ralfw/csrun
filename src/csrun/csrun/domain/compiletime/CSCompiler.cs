using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using csrun.data.domain;
using csrun.domain.runtime;
using Microsoft.CSharp;
using CompilerError = csrun.data.domain.CompilerError;

namespace csrun.domain.compiletime
{
    internal class CSCompiler
    {
        public static void Compile(Sourcecode csSource, Action<Executable> onSuccess, Action<IEnumerable<CompilerError>> onFailure)
        {
            var cr = Compile(csSource);

            if (cr.Errors.HasErrors) {
                onFailure(MapErrors(cr.Errors));
                return;
            }

            onSuccess(new Executable(cr.CompiledAssembly));
        }


        static CompilerResults Compile(Sourcecode csSource) {
            var provider = new CSharpCodeProvider();

            var exePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            
            var cp = new CompilerParameters();
            cp.ReferencedAssemblies.Add( "System.dll" );
            cp.ReferencedAssemblies.Add( "System.Core.dll" );
            cp.ReferencedAssemblies.Add( "System.Xml.dll" );
            cp.ReferencedAssemblies.Add( "System.Web.Extensions.dll" );
            cp.ReferencedAssemblies.Add(Path.Combine(exePath, "System.ValueTuple.dll"));
            cp.ReferencedAssemblies.Add(Path.Combine(exePath, "nunit.framework.dll"));
            cp.GenerateExecutable = true;
            cp.OutputAssembly = Path.GetFileNameWithoutExtension(csSource.Filename) + ".exe";
            cp.GenerateInMemory = false;
            cp.IncludeDebugInformation = true;
            
            File.WriteAllLines(csSource.Filename, csSource.Text);
            return provider.CompileAssemblyFromFile(cp, csSource.Filename);
        }

        
        static IEnumerable<CompilerError> MapErrors(CompilerErrorCollection errors) {
            foreach (System.CodeDom.Compiler.CompilerError ce in errors) {
                yield return new csrun.data.domain.CompilerError {
                    Filename = Path.GetFileName(ce.FileName),
                    LineNumber = ce.Line,
                    ColumnNumber = ce.Column,
                    Description = ce.IsWarning ? "Warning" : "Error" + $": {ce.ErrorText}"
                };
            }
        }
    }
}