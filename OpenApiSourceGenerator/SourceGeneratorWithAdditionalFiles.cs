using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using NSwag;
using NSwag.CodeGeneration.CSharp;

namespace OpenApiGenerator;

[Generator]
public class SourceGeneratorWithAdditionalFiles : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var files = context.AdditionalTextsProvider
            .Where(m => m is not null)
            .Where(a => Path.GetFileName(a.Path).EndsWith(".json"))
            .Select((a, c) => (FileName: Path.GetFileNameWithoutExtension(a.Path), Content: a.GetText(c)!.ToString()))
            .Collect();

        var compilationAndFiles = context.CompilationProvider.Combine(files);
        
        //context.RegisterSourceOutput(compilationAndFiles,
        //    (productionContext, sourceContext) => GenerateCode(productionContext, sourceContext.Left, sourceContext.Right));
        
        context.RegisterSourceOutput(files, GenerateCode); 
    }

    private static void GenerateCode(
        SourceProductionContext context,
        ImmutableArray<(string FileName, string Content)> files)
    {
        var document = GenerateDocument(files
            .Where(f => f.FileName == "WolfApi")
            .Select(f => f.Content)
            .First()
        );

        var documentPatches = files
            .Where(f => f.FileName != "WolfApi")
            .Select(f => f.Content)
            .Select(GenerateDocument);

        foreach (var patch in documentPatches)
        {
            if(patch is null)
                continue;

            foreach (var kv in patch.Paths)
            { 
                document.Paths[kv.Key] = kv.Value;
            }
            foreach (var kv in patch.Parameters)
            { 
                document.Parameters[kv.Key] = kv.Value;
            }
            foreach (var kv in patch.Definitions)
            { 
                document.Definitions[kv.Key] = kv.Value;
            }
            foreach (var kv in patch.Responses)
            { 
                document.Responses[kv.Key] = kv.Value;
            }
            foreach (var kv in patch.SecurityDefinitions)
            { 
                document.SecurityDefinitions[kv.Key] = kv.Value;
            }
        }
        
        var settings = new CSharpClientGeneratorSettings
        {
            ClassName = "GeneratedApiClient", 
            CSharpGeneratorSettings = 
            {
                Namespace = "GamesOnWhales"
            }
        };
        
        var generator = new CSharpClientGenerator(document, settings);	
        var code = generator.GenerateFile();
        context.AddSource($"GeneratedApiClient.g.cs", code);
    }

    private static OpenApiDocument GenerateDocument(string jsonText)
    { 
        var text = jsonText;    
        // replaces all type null with nullable:true, needs more cleanup after
        text = Regex.Replace(text, """{"type":"null"}""", """{"nullable":true}""");
        // removes anyOf if only one type and nullable:true are inside.
        text = Regex.Replace(text, @"""anyOf"":\[{""type"":""([aA-zZ]+)""},{""nullable"":true}]", @"""type"":""$1"",""nullable"":true");
        // remove empty required arrays
        text = Regex.Replace(text, @",\""required\"":\[]", "");
        // change number string to number value
        text = Regex.Replace(text, @"""(min|max)Items"":""([0-9]+)""", @"""$1Items"":$2");
        // patching in the response content for get-icon
        text = Regex.Replace(text, @"(v1\/utils\/get-icon"":{.*?,""responses"":{)}", 
            @"$1""200"":{""content"":{""image/png"":{""schema"":{""type"":""string"",""format"":""binary""}}},""description"":""""}}");
        // force runner to be docker runner only, multiple different objects aren't possible  
        text = Regex.Replace(text, @"(""runner"":{""anyOf"":\[.*?])}", @"""runner"":{""$ref"":""#/components/schemas/wolf__config__AppDocker__tagged""}");
        // removes the enum from type, since only docker is used here.
        text = Regex.Replace(text, @"""type"":{""type"":""string"",""enum"":\[.*?]}", @"""type"":{""type"":""string""}");
        // Make PartialClientSettings correctly optional
        text = Regex.Replace(text, @"""(client_settings|settings)"":{""description"":"".*?"",""anyOf"":\[{""\$ref"":""(.*?)""},{""nullable"":true}]}", @"""$1"":{""nullable"":true,""allOf"":[{""$ref"":""$2""}]}");
        // fixes optional Pin fields, since they shouldn't be "anyOf"
        text = Regex.Replace(text, @"""anyOf"":\[{""type"":""array"",""items"":{""type"":""integer""}},{""nullable"":true}]", @"""type"":""array"",""items"":{""type"":""integer""},""nullable"":true");
        // sanitize reflection type names.
        text = Regex.Replace(text, @"rfl__Reflector_wolf__core__events__(.*?)___ReflType", @"$1");
        // sanitize endpoint return type names
        text = Regex.Replace(text, @"wolf__api__(.*?)""", @"$1""");
        // sanitize events type names
        text = Regex.Replace(text, @"wolf__core__events__(.*?)""", @"$1""");
        return OpenApiDocument.FromJsonAsync(text).Result;
    }

    private static void GenerateClientCode(SourceProductionContext context,
        string attributeArgument,
        INamedTypeSymbol typeSymbol,
        string openApiJson)
    {
        try
        {
            var document = GenerateDocument(openApiJson);
            
            var settings = new CSharpClientGeneratorSettings
            {
                ClassName = typeSymbol.Name, 
                CSharpGeneratorSettings = 
                {
                    Namespace = typeSymbol.ContainingNamespace.Name
                }
            };
        
            var generator = new CSharpClientGenerator(document, settings);	
            var code = generator.GenerateFile();
            context.AddSource($"{typeSymbol.ToDisplayString()}.{attributeArgument}.new.g.cs", code);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    
    private static async void GenerateCode(SourceProductionContext context, Compilation compilation,
        ImmutableArray<(string, string)> files)
    {
        foreach (var file in files)
        {
            try
            {
                var document = GenerateDocument(file.Item2);

                var settings = new CSharpClientGeneratorSettings
                {
                    ClassName = "NSwag" + Path.GetFileNameWithoutExtension(file.Item1), 
                    CSharpGeneratorSettings = 
                    {
                        Namespace = "NSwag" + Path.GetFileNameWithoutExtension(file.Item1)
                    }
                };
            
                var generator = new CSharpClientGenerator(document, settings);	
                var code = generator.GenerateFile();
                context.AddSource($"{"NSwag" + Path.GetFileNameWithoutExtension(file.Item1)}.g.cs", code);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}