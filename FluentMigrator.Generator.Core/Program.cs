// See https://aka.ms/new-console-template for more information

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SF = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

var name = args.AsQueryable().FirstOrDefault();

var @namsepace = "Migrations";

if (string.IsNullOrEmpty(name))
{
    Console.Write("Name:");
    name = Console.ReadLine();
}




if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(@namsepace))
{
    var syntaxFactory = SF.CompilationUnit();

    var @namespace = SF.NamespaceDeclaration(SF.ParseName(@namsepace))
        .NormalizeWhitespace();
    
    syntaxFactory = syntaxFactory.AddUsings(
        SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("FluentMigrator")));

    var date = DateTime.Now.Ticks;
    
    var attributeArgument = SyntaxFactory.AttributeArgument(
        null, null, SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression,
            SyntaxFactory.Literal(date)));
    
    var classDeclaration = SF.ClassDeclaration(name)
        .AddModifiers(SF.Token(SyntaxKind.PublicKeyword))
        .AddBaseListTypes(
        SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName("Migration")))
        .AddAttributeLists(SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(
            SyntaxFactory.Attribute(SyntaxFactory.IdentifierName("Migration"))
                .WithArgumentList(SyntaxFactory.AttributeArgumentList(SyntaxFactory.SingletonSeparatedList(attributeArgument)))
        )));

    var syntaxUp = SyntaxFactory.ParseStatement("");
    var syntaxDown = SyntaxFactory.ParseStatement("");

    var methodUp = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName("void"), "Up")
        .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.OverrideKeyword))
        .WithBody(SyntaxFactory.Block(syntaxUp));

    var methodDown = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName("void"), "Down")
        .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.OverrideKeyword))
        .WithBody(SyntaxFactory.Block(syntaxDown));
    
    
    
    classDeclaration = classDeclaration.AddMembers(methodUp, methodDown);
    
    @namespace = @namespace.AddMembers(classDeclaration);

    // Add the namespace to the compilation unit.
    syntaxFactory = syntaxFactory.AddMembers(@namespace);
    
    var code = syntaxFactory
        .NormalizeWhitespace()
        .ToFullString();
    
    Console.WriteLine(code);
    
}
else
{
    Console.WriteLine("Requirements not met");
}