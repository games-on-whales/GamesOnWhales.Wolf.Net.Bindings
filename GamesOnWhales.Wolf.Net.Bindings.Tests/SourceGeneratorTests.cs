using System.Collections.Immutable;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using OpenApiGenerator;
using Xunit;
using Xunit.Internal;

namespace GamesOnWhales.Wolf.Net.Bindings.Tests;

public class SourceGeneratorTests
{
    private static Compilation CreateCompilation(string source)
        => CSharpCompilation.Create("compilation",
            [CSharpSyntaxTree.ParseText(source)],
            [MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location)],
            new CSharpCompilationOptions(OutputKind.ConsoleApplication));
    
    [Fact]
    public void SourceGeneratorDebugTest()
    {
        // Create an instance of the source generator.
        var generator = new SourceGeneratorWithAdditionalFiles();
        var notifyGenerator = new SseEventHandlerGenerator();
        
        // Source generators should be tested using 'GeneratorDriver'.
        GeneratorDriver driver = CSharpGeneratorDriver.Create(notifyGenerator, generator);
        
        // Add the additional file separately from the compilation.
        driver = driver.AddAdditionalTexts(
            [
                new TestAdditionalFile("./WolfApi.json", "GamesOnWhales.Wolf.Net.Bindings.Tests.WolfApi.json"),
                new TestAdditionalFile("./Docker.json", "GamesOnWhales.Wolf.Net.Bindings.Tests.Docker.json")
            ]
        );

        var compilation = CreateCompilation("""
                                            namespace GamesOnWhales;
                                            
                                            public interface ISseEventHandler
                                            {
                                                public Task Call(WolfApi api, string eventData);
                                                string EventName { get; }
                                            }
                                            
                                            public interface ISseEventHandler<T> : ISseEventHandler
                                            {
                                                public Task Convert(string eventData, out T result);
                                            }
                                            
                                            public class PlugDeviceEvent
                                            {
                                                
                                            }
                                            
                                            [SseEventHandler]
                                            public partial class PlugDeviceEventHandler : ISseEventHandler<PlugDeviceEvent>
                                            {
                                                public Task Call(WolfApi api, string eventData)
                                                {
                                                    
                                                    return Task.CompletedTask;
                                                }
                                            
                                                public Task Convert(string eventData, out PlugDeviceEvent result)
                                                {
                                                    throw new NotImplementedException();
                                                }
                                            
                                                public string EventName => "wolf::core::events::PlugDeviceEvent";
                                            }
                                            """);
        
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var newCompilation, out _, TestContext.Current.CancellationToken);

        // Retrieve all files in the compilation.
        var generatedFiles = newCompilation.SyntaxTrees
            .Select(t => (Name: Path.GetFileName(t.FilePath), Content: t.GetText().ToString()))
            .ToArray();
        
        // Assert.Equivalent(new[]
        // {
        // 
        // }, generatedFiles);
    }
}