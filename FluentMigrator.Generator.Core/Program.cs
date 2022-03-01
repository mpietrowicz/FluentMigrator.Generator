// See https://aka.ms/new-console-template for more information

using System.Text.RegularExpressions;
using FluentMigrator.Generator.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using SF = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

MigrationsBuilder.Instance.Init(args);

var directory = Directory.GetCurrentDirectory();

var directoryOfMigrations = Path.Combine(directory, "Migrations");

Console.WriteLine(directory);

var nameOfMigration = args?.AsQueryable()?.FirstOrDefault() ?? String.Empty;

var namespaceOfMigration = "Migrations";

if (string.IsNullOrEmpty(nameOfMigration))
{
    Console.Write("Name:");
    nameOfMigration = Console.ReadLine();
    nameOfMigration = RemoveInvalidFilePathCharacters(nameOfMigration,"");

}



if (!string.IsNullOrEmpty(nameOfMigration) && !string.IsNullOrEmpty(namespaceOfMigration))
{
    var syntaxFactory = SF.CompilationUnit();

    var @namespace = SF.NamespaceDeclaration(SF.ParseName(namespaceOfMigration))
        .NormalizeWhitespace();
    
    syntaxFactory = syntaxFactory.AddUsings(
        SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("FluentMigrator")));

    var date = DateTime.Now.ToString("ddMMyyyyHHmmss");
    
    var attributeArgument = SyntaxFactory.AttributeArgument(
        null, null, SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression,
            SyntaxFactory.Literal(date)));
    
    var classDeclaration = SF.ClassDeclaration(nameOfMigration)
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
    
   

    if (!Directory.Exists(directoryOfMigrations))
    {
        Directory.CreateDirectory(directoryOfMigrations);
    }
    var filePathToMigration = Path.Combine(directoryOfMigrations, $"{date}_{nameOfMigration}.cs");
    if (File.Exists(filePathToMigration))
    {
        Console.Write($"File Exists in path {filePathToMigration} overite  y/n (def n)? :");

        var overite = Console.ReadLine().ToLower().Trim() == "y" ? true : false;
        if (!overite)
        {
            Console.WriteLine("Aborting!");
            return;
        }
    }
    File.WriteAllText(filePathToMigration,code);
    Console.WriteLine(code);
    
    
}
else
{
    Console.WriteLine("Requirements not met");
}


string RemoveInvalidFilePathCharacters(string filename, string replaceChar)
{
    if (filename == null) throw new ArgumentNullException(nameof(filename));
    if (replaceChar == null) throw new ArgumentNullException(nameof(replaceChar));
    string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
    Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
    return r.Replace(filename, replaceChar).Replace(" ",replaceChar);
}