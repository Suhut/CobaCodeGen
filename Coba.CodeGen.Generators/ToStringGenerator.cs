using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using System.Text;

namespace Coba.CodeGen.Generators;

[Generator]
public sealed class ToStringGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classes = context.SyntaxProvider.CreateSyntaxProvider(
                predicate: static (node, _) => IsSyntaxTarget(node),
                transform: static (ctx, _) => GetSemanticTarget(ctx)
            ).Where(static x => x is not null);

        context.RegisterSourceOutput(classes,
                  static (ctx, source) => Execute(ctx, source));

        context.RegisterPostInitializationOutput(
            static (ctx) => PostInitializationOutout(ctx)
            );
    }


    private static bool IsSyntaxTarget(SyntaxNode node)
    {
        return node is ClassDeclarationSyntax classDeclarationSyntax && classDeclarationSyntax.AttributeLists.Count > 0;
    }
    private static ClassDeclarationSyntax GetSemanticTarget(GeneratorSyntaxContext context)
    {
        var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;

        var result = (
                from T0 in classDeclarationSyntax.AttributeLists
                from T0_T0 in T0.Attributes
                where T0_T0.Name.ToString().Equals("GenerateToString") || T0_T0.Name.ToString().Equals("GenerateToStringAttribute")
                select T0_T0
                ).Any();

        if (result)
        {
            return classDeclarationSyntax;
        }
        else
        {
            return null;
        }
    }

    private static void PostInitializationOutout(IncrementalGeneratorPostInitializationContext context)
    {
        var fileName = $"Coba.CodeGen.Generators.GenerateToStringAttribute.g.cs";

        var stringBuilder = new StringBuilder();
        stringBuilder.Append($"""
            namespace Coba.CodeGen.Generators;
            
            internal class GenerateToStringAttribute : System.Attribute
            {"{"}
               
            {"}"} 
            """);
        context.AddSource(fileName, stringBuilder.ToString());
    }

    private static void Execute(SourceProductionContext context, ClassDeclarationSyntax classDeclarationSyntax)
    {
        if (classDeclarationSyntax.Parent
            is BaseNamespaceDeclarationSyntax namespaceDeclarationSyntax)
        {

            var namespaceName = namespaceDeclarationSyntax.Name.ToString();
            var className = classDeclarationSyntax.Identifier.Text;
            var fileName = $"{namespaceName}.{className}.g.cs";

            ////TODO: Remove later when attribute is checked
            //if (className.Equals("GenerateToStringAttribute", StringComparison.OrdinalIgnoreCase))
            //{
            //    return;
            //}

            var props = classDeclarationSyntax.Members
                                .OfType<PropertyDeclarationSyntax>()
                                .Where(x => x.Modifiers.Any(SyntaxKind.PublicKeyword))
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