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
    public class Generator: IDisposable
    {
        private int maxCountOfThreads;
        private  ConcurrentBag<CompilationUnitSyntax> result;
        private TypeConverter typeConverter;
        private CountdownEvent handleFinishEvent;
        private string classesNamespace;
        private bool disposed = false;

        public Generator(int maxCountOfThreads, string classesNamespace)
        {
        
            typeConverter = TypeConverter.Instance;
            this.maxCountOfThreads = maxCountOfThreads;
            this.classesNamespace = classesNamespace; 
            result = new ConcurrentBag<CompilationUnitSyntax>();
        }

        public Dictionary<string, CompilationUnitSyntax> GenerateDtoList(List<ClassInfo> classInfoList)
        {
            ThreadPool.SetMaxThreads(maxCountOfThreads, maxCountOfThreads);
            Dictionary<string, CompilationUnitSyntax> resultDictionary = new Dictionary<string, CompilationUnitSyntax>();

            handleFinishEvent = new CountdownEvent(classInfoList.Count);

            foreach (ClassInfo classInfo in classInfoList)
            {
                ThreadPool.QueueUserWorkItem(GenerateDto, classInfo);
            }

            handleFinishEvent.Wait();

            foreach (CompilationUnitSyntax syntax in result)
            {
                resultDictionary.Add(GetClassNameFromSyntax(syntax), syntax);
            }         
            return resultDictionary;    
        }

        private void GenerateDto(object classInfo)
        {
            ClassInfo info = (ClassInfo)classInfo;
            CompilationUnitSyntax compilationUnitSyntax = SyntaxFactory.CompilationUnit()
                .AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName("System")))
                .AddMembers(SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName(classesNamespace))
                .AddMembers(GenerateClass(info)));
            result.Add(compilationUnitSyntax);
            handleFinishEvent.Signal();
        }

        private ClassDeclarationSyntax GenerateClass(ClassInfo classInfo)
        {
            ClassDeclarationSyntax classDeclarationSyntax = SyntaxFactory.ClassDeclaration(SyntaxFactory.Identifier(classInfo.ClassName))
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddMembers(GeneratePropertyList(classInfo.Properties));
            return classDeclarationSyntax;
        }

        private PropertyDeclarationSyntax[] GeneratePropertyList(List<PropertyInfo> propertyInfoList)
        {
            PropertyDeclarationSyntax[] properties = new PropertyDeclarationSyntax[propertyInfoList.Count];
            int i = 0;
            foreach (PropertyInfo propertyInfo in propertyInfoList)
            {
                properties[i++] = GenerateProperty(propertyInfo);
            }
            return properties;
        }

        private PropertyDeclarationSyntax GenerateProperty(PropertyInfo propertyInfo)
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

        private TypeSyntax GenerateType(string type, string format)
        {
            string netTypeName = typeConverter.TryGetTypeName(new TypeInfo(type, format));
            return SyntaxFactory.ParseTypeName(netTypeName);
        }

        private string GetClassNameFromSyntax(CompilationUnitSyntax syntax)
        {
            return ((ClassDeclarationSyntax)(((NamespaceDeclarationSyntax)syntax.Members[0]).Members[0])).Identifier.ToString();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    typeConverter = null;
                }
                
                result = null;
                handleFinishEvent = null;
                disposed = true;
            }
        }

        ~Generator()
        {
            Dispose(disposed);
        }
    }
}
