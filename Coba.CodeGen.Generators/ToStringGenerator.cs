using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using System.Text;

namespace Coba.CodeGen.Generators;

[Generator]
public sealed class ToStringGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classes = context.SyntaxProvider.CreateSyntaxProvider(
                predicate: static (node, _) => node is ClassDeclarationSyntax,
                transform: static (ctx, _) => (ClassDeclarationSyntax)ctx.Node
            );
        context.RegisterSourceOutput(classes,
                  static (ctx, source) => Execute(ctx, source));
    }

    private static void Execute(SourceProductionContext context, ClassDeclarationSyntax classDeclarationSyntax)
    {
        if (classDeclarationSyntax.Parent
            is BaseNamespaceDeclarationSyntax namespaceDeclarationSyntax)
        {

            var namespaceName = namespaceDeclarationSyntax.Name.ToString();
            var className = classDeclarationSyntax.Identifier.Text;
            var fileName = $"{namespaceName}.{className}.g.cs"; 
            
            var props = classDeclarationSyntax.Members
                                .OfType<PropertyDeclarationSyntax>()
                                .Where(x=>x.Modifiers.Any(SyntaxKind.PublicKeyword))
                                .Select(x => $"{x.Identifier.Text}:{{{x.Identifier.Text}}}")
                                .ToArray()
                               ;   

            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"""
            namespace {namespaceName};
            
            public partial class {className} 
            {"{"}
                public override string ToString() 
                {"{"}
                    return $"{string.Join("; ", props)}";
                {"}"}  
            {"}"} 
            """);
            context.AddSource(fileName, stringBuilder.ToString());

        }
    }
}