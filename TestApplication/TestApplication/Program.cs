using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using DtoGenerator;
using System.Configuration;
using System.IO;

using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Concurrent;

namespace TestApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            int maxCountOfThreads = int.Parse(ConfigurationManager.AppSettings["threadNumber"]);
            string classesNamespace = ConfigurationManager.AppSettings["classesNamespace"];
            string directoryName = args[0];
            string classesFileName = args[1];
            List<ClassInfo> classInfoList = JsonClassReader.ReadClassInfo(classesFileName);
            Generator generator = new Generator(maxCountOfThreads, classesNamespace);
            Dictionary<string, CompilationUnitSyntax> result = generator.GenerateDtoList(classInfoList);
            ConcurrentDictionary<string, CompilationUnitSyntax> dictionary = new ConcurrentDictionary<string, CompilationUnitSyntax>();
            foreach(String name in result.Keys)
            {
                dictionary.TryAdd(name, result[name]);
            }
            SyntaxWriter writer = new SyntaxWriter(directoryName);
            writer.WriteSyntaxList(dictionary);

        }
    }
}
