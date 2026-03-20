using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace OpenApiGenerator;

[Generator]
public class SseEventHandlerGenerator : IIncrementalGenerator
{
    private const string NotifyAttributeSourceCode = """
                                                     namespace GamesOnWhales
                                                     {
                                                         [global::System.AttributeUsage(global::System.AttributeTargets.Class)]
                                                         internal class SseEventHandlerAttribute : Attribute
                                                         { }
                                                     }
                                                     """;
    
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
             "NotifyEventAttribute.g.cs",
             SourceText.From(NotifyAttributeSourceCode, Encoding.UTF8)));

        var notify = context.SyntaxProvider.ForAttributeWithMetadataName("GamesOnWhales.SseEventHandlerAttribute",
                predicate: (c, _) => c is ClassDeclarationSyntax,
                transform: (n, _) => n.TargetNode as ClassDeclarationSyntax)
            .Where(m => m is not null)
            .Select((n, _) => n!);
        var compilation = context.CompilationProvider.Combine(notify.Collect());

        context.RegisterSourceOutput(compilation,
            (spc, source) => Execute(spc, source.Left, source.Right));
    }

    private void Execute(SourceProductionContext context, Compilation compilation,
        ImmutableArray<ClassDeclarationSyntax> syntaxes)
    {
        foreach (var syntax in syntaxes)
        {
            if (ModelExtensions.GetDeclaredSymbol(compilation
                        .GetSemanticModel(syntax.SyntaxTree), syntax) is not INamedTypeSymbol symbol)
                continue;
            
            
            var className = symbol.Name;
            if (!className.EndsWith("EventHandler"))
            {
                return;
            }

            var eventName = className.Substring(0, className.Length - "Handler".Length);

            var interfaceGenericType = "string";
            foreach (var interfaceSymbol in symbol.Interfaces.Where(interfaceSymbol => interfaceSymbol.Name == "ISseEventHandler"))
            {
                interfaceGenericType = interfaceSymbol.TypeArguments[0].ToDisplayString();
            }
            
            var namespaceName = symbol.ContainingNamespace.ToDisplayString();

            var simplePassthrough = """
                                            public Task Convert(string eventData, out string result)
                                            {
                                                if(Logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Debug))
                                                    Logger.LogDebug("{event}: {data}", EventName, eventData);
                                                result = eventData;
                                                return Task.CompletedTask;
                                            }
                                    """;

            var jsonConvert = $$"""
                                #nullable enable
                                        public Task Convert(string eventData, out {{interfaceGenericType}} result)
                                        {
                                            try
                                            {
                                                result = System.Text.Json.JsonSerializer.Deserialize<{{interfaceGenericType}}>(eventData);
                                            }
                                            catch(System.Text.Json.JsonException)
                                            {
                                                Logger.LogError("failed converting event: {event} to type {{interfaceGenericType}}:\n{data}", EventName, eventData);
                                                result = null;
                                            }

                                            return Task.CompletedTask;
                                        }
                                #nullable disable
                                """;
            
            var code = $$"""
                       #pragma warning disable CS9113
                       using Microsoft.Extensions.Logging;
                       
                       namespace GamesOnWhales
                       {
                            public partial class WolfApi
                            {
                       #nullable enable
                                public event Func<object, {{interfaceGenericType}}, System.Threading.Tasks.Task>? {{eventName}};
                       
                                public async Task Emit{{eventName}}({{interfaceGenericType}} data)
                                {
                                    await On{{eventName}}(data);
                                    await Emit({{eventName}}, data);
                                }
                                
                                protected virtual Task On{{eventName}}({{interfaceGenericType}} data)
                                    => Task.CompletedTask;

                       #nullable disable
                            }
                       }
                       
                       namespace {{namespaceName}}
                       {
                            /// <summary>EventHandler for a specific SSE event.
                            /// Register the EventHandler to enable listening to it's associated SSE event, 
                            /// or pass it to the constructor of the <c>WolfApi</c> manually if no DI framework is used.</summary>
                            public partial class {{className}}
                            {
                                public Microsoft.Extensions.Logging.ILogger Logger { get; }
                                public {{className}}(Microsoft.Extensions.Logging.ILogger<{{className}}> logger)
                                {
                                    Logger = logger;
                                }
                                
                                public async Task Call(WolfApi api, string data)
                                {
                                    await Convert(data, out var converted);
                                    await api.Emit{{eventName}}(converted);
                                }
                       {{(interfaceGenericType == "string" ? simplePassthrough : jsonConvert)}}
                            }
                       }
                       #pragma warning restore CS9113
                       """;
            
            context.AddSource($"{eventName}.g.cs", code);
        }
    }
}