using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Concurrent;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.MSBuild;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TestApplication
{
    class SyntaxWriter
    {
        private string extension = ".cs";
        private string directory;

        public SyntaxWriter(string directory)
        {
            this.directory = directory;
        }

        public void WriteSyntaxList(ConcurrentDictionary<string, CompilationUnitSyntax> syntaxDictionary)
        {
            foreach (string key in syntaxDictionary.Keys)
            {
                WriteSyntax(key, syntaxDictionary[key]);
            }
        }

        public string GenerateCodeString(CompilationUnitSyntax syntax)
        {
            SyntaxNode formattedNode = Formatter.Format(syntax, new AdhocWorkspace());
            StringBuilder sb = new StringBuilder();
            using (StringWriter writer = new StringWriter(sb))
            {
                formattedNode.WriteTo(writer);
            }
            return sb.ToString();
        }

        private string GeneratePathToFile(string name)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), directory, name + extension);
        }

        public void WriteSyntax(string name, CompilationUnitSyntax syntax)
        {
            using (TextWriter writer = File.CreateText(GeneratePathToFile(name)))
            {
                writer.Write(GenerateCodeString(syntax));
            }
        }
    }
}
