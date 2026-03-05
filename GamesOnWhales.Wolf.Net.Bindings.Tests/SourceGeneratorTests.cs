using System.Collections.Immutable;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using OpenApiGenerator;
using Xunit;

namespace GamesOnWhales.Wolf.Net.Bindings.Tests;

public class SourceGeneratorTests
{
    private const string DddRegistryText = @"User
Document
Customer";

    [Fact]
    public void GenerateClassesBasedOnDDDRegistry()
    {
        // Create an instance of the source generator.
        var generator = new SourceGeneratorWithAdditionalFiles();
        
        // Source generators should be tested using 'GeneratorDriver'.
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
        
        // Add the additional file separately from the compilation.
        driver = driver.AddAdditionalTexts(
            [
                new TestAdditionalFile("./WolfApi.json", "GamesOnWhales.Wolf.Net.Bindings.Tests.WolfApi.json"),
                new TestAdditionalFile("./Docker.json", "GamesOnWhales.Wolf.Net.Bindings.Tests.Docker.json")
            ]
        );

        // To run generators, we can use an empty compilation.
        var compilation = CSharpCompilation.Create(nameof(SourceGeneratorTests));

        // Run generators. Don't forget to use the new compilation rather than the previous one.
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var newCompilation, out _, TestContext.Current.CancellationToken);

        // Retrieve all files in the compilation.
        var generatedFiles = newCompilation.SyntaxTrees
            .Select(t => Path.GetFileName(t.FilePath))
            .ToArray();
        
        Assert.Equivalent(new[]
        {
            "GeneratedApiClient.g.cs"
        }, generatedFiles);
    }
}