using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace GamesOnWhales.Wolf.Net.Bindings.Tests;

public class TestAdditionalFile : AdditionalText
{
    private readonly SourceText _text;

    public TestAdditionalFile(string path, string name)
    {
        var assembly = typeof(TestAdditionalFile).GetTypeInfo().Assembly;
        var resource = assembly.GetManifestResourceStream(name);

        if (resource == null)
        {
            throw new Exception("Resource not found");
        }
        
        var ms = new MemoryStream();
        resource.CopyTo(ms);
        var fileBuffer = ms.ToArray();
        var s = System.Text.Encoding.Default.GetString(fileBuffer);
        
        Path = path;
        _text = SourceText.From(s);
    }

    public override SourceText GetText(CancellationToken cancellationToken = new()) => _text;

    public override string Path { get; }
}