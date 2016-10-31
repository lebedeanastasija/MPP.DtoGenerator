using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using DtoGenerator;

using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Concurrent;

namespace TestApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            List<ClassInfo> classInfoList = JsonClassReader.ReadClassInfo("info.json");
            Generator generator = new Generator();
            Dictionary<string, CompilationUnitSyntax> result = generator.GenerateDtoList(classInfoList);
            ConcurrentDictionary<string, CompilationUnitSyntax> dictionary = new ConcurrentDictionary<string, CompilationUnitSyntax>();
            foreach(String name in result.Keys)
            {
                dictionary.TryAdd(name, result[name]);
            }
            SyntaxWriter writer = new SyntaxWriter("generated_classes");
            writer.WriteSyntaxList(dictionary);

        }
    }
}
