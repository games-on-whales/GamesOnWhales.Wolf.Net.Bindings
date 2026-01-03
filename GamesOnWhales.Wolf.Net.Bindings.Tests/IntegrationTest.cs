using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Testing;
using Org.BouncyCastle.Asn1.Anssi;
using Xunit.Abstractions;

namespace GamesOnWhales.Wolf.Net.Bindings.Tests;

public sealed class IntegrationTest : IClassFixture<WolfContainer>
{
    private WolfApi Api { get; }
    
    [Fact]
    public async Task TestGetProfiles()
    {
        var response = await Api.GetProfiles();
        Assert.Contains(response, profile => profile.Name == "User");
        var resp = await Api.GeneratedClient.ProfilesAsync();
        Assert.Contains(resp.Profiles, profile => profile.Name == "User");
    }

    [Fact]
    public async Task TestGetUtilsGetIcon()
    {
        var imgStream = await Api.GetUtilsGetIconAsync("/etc/wolf/test.png");
        Assert.NotNull(imgStream);
        
        // Check if test image uploaded to the Test container gets send correctly via the Api.
        var assembly = typeof(IntegrationTest).GetTypeInfo().Assembly;
        var resource = assembly.GetManifestResourceStream("GamesOnWhales.Wolf.Net.Bindings.Tests.test.png");
        Assert.NotNull(resource);

        var ms1 = new MemoryStream();
        var ms2 = new MemoryStream();
        await resource.CopyToAsync(ms1);
        await imgStream.CopyToAsync(ms2);
        
        Assert.Equivalent(ms1.ToArray(), ms2.ToArray(), strict: true);
    }
    
    // Sadly mounting the docker socket wont allow the wolf container to run anymore, reason still unkown
    // TODO: Fix mounting docker socket allowing for tests that rely on docker access...
    //[Fact]
    //public async Task TestDockerInspect()
    //{
    //     var response = await Api.GetDockerImagesInspectAsync("ghcr.io/games-on-whales/wolf:stable");
    //     testOutputHelper.WriteLine(response.ToString());
    //}

    #region SetupTestEnvironment
    // Lifetime of the Container should be the same as the Classes Object.
    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private readonly WolfContainer _container;
    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private readonly ITestOutputHelper _output;
    private FakeLogCollector LogCollector { get; set; }
    public IntegrationTest(ITestOutputHelper testOutputHelper, WolfContainer container)
    {
        _container = container;
        _output = testOutputHelper;
        
        LogCollector = FakeLogCollector.Create(new FakeLogCollectorOptions
        {
            OutputSink = message =>
            {
                try
                {
                    testOutputHelper.WriteLine(message);
                }
                catch
                {
                    // ignored, because testOutputHelper throws after the test is finished
                }
            }
        });

        // Reroute Container log output.
        container.LogAction = message =>
        {
            try
            {
                _output.WriteLine($"[WolfContainer] {message}");
            }
            catch
            {
                // ignored, because testOutputHelper throws after the test is finished
            }
        };

        var outs = _container.TestContainer.GetLogsAsync().GetAwaiter().GetResult();
        outs.Stderr.Split("\n").ToList().ForEach(line => _output.WriteLine($"[WolfContainer] {line}"));
        
        var tmpFolder = Path.GetTempPath();
        var path = Path.Join(tmpFolder, "GamesOnWhales.Wolf.Net.Bindings.Tests");
        
        Api = new WolfApi(new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                {"SOCKET_PATH", $"unix://{Path.Join(path, "cfg/wolf.sock")}"}
            }!).Build());
    }
    #endregion
}