﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DtoGenerator
{
    public class Generator
    {
        private int maxCountOfThreads;
        private static ConcurrentBag<CompilationUnitSyntax> result;
        private static TypeConverter typeConverter;

        public Generator(int maxCountOfThreads = 5)
        {
            typeConverter = TypeConverter.Instance;
            this.maxCountOfThreads = maxCountOfThreads;
            result = new ConcurrentBag<CompilationUnitSyntax>();
        }

        public Dictionary<string, CompilationUnitSyntax> GenerateDtoList(List<ClassInfo> classInfoList)
        {
            ThreadPool.SetMaxThreads(maxCountOfThreads, maxCountOfThreads);
            Dictionary<string, CompilationUnitSyntax> resultDictionary = new Dictionary<string, CompilationUnitSyntax>();

            foreach (ClassInfo classInfo in classInfoList)
            {
                ThreadPool.QueueUserWorkItem(GenerateDto, classInfo);
            }

            foreach (CompilationUnitSyntax syntax in result)
            {
                resultDictionary.Add(GetNameFromCompilationUnitSyntax(syntax), syntax);
            }         
            return resultDictionary;    
        }

        private static void GenerateDto(object classInfo)
        {
            ClassInfo info = (ClassInfo)classInfo;
            CompilationUnitSyntax compilationUnitSyntax = SyntaxFactory.CompilationUnit()
                .AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName("System")))
                .AddMembers(GenerateClass(info));
            result.Add(compilationUnitSyntax);
        }

        private static  ClassDeclarationSyntax GenerateClass(ClassInfo classInfo)
        {
            ClassDeclarationSyntax classDeclarationSyntax = SyntaxFactory.ClassDeclaration(SyntaxFactory.Identifier(classInfo.ClassName))
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddMembers(GeneratePropertyList(classInfo.Properties));
            return classDeclarationSyntax;
        }

        private static PropertyDeclarationSyntax[] GeneratePropertyList(List<PropertyInfo> propertyInfoList)
        {
            PropertyDeclarationSyntax[] properties = new PropertyDeclarationSyntax[propertyInfoList.Count];
            int i = 0;
            foreach (PropertyInfo propertyInfo in propertyInfoList)
            {
                properties[i++] = GenerateProperty(propertyInfo);
            }
            return properties;
        }

        private static PropertyDeclarationSyntax GenerateProperty(PropertyInfo propertyInfo)
        {
            TypeSyntax type = GenerateType(propertyInfo.Type, propertyInfo.Format);
            PropertyDeclarationSyntax property = SyntaxFactory.PropertyDeclaration(type, propertyInfo.Name)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddAccessorListAccessors(SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).
                    WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).
                    WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));
            return property;
        }

        private static TypeSyntax GenerateType(string type, string format)
        {
            string _NetTypeName = typeConverter.TryGetTypeName(new TypeInfo(type, format));
            return SyntaxFactory.ParseTypeName(_NetTypeName);
        }

        private static string GetNameFromCompilationUnitSyntax(CompilationUnitSyntax syntax)
        {
            return ((ClassDeclarationSyntax)syntax.Members[0]).Identifier.ToString();
        }
    }
}