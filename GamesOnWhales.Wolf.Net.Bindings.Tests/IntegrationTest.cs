using Microsoft.Extensions.Configuration;
using Xunit.Abstractions;

namespace GamesOnWhales.Wolf.Net.Bindings.Tests;

public class IntegrationTest(ITestOutputHelper testOutputHelper, WolfContainer container) : IAsyncLifetime, IClassFixture<WolfContainer>
{
    private WolfApi Api { get; set; } = null!;
    
    [Fact]
    public async Task TestGetProfiles()
    {
        var response = await Api.GetProfiles();
        Assert.Contains(response, profile => profile.Name == "User");
        var resp = await Api.GeneratedApiClient.ProfilesAsync();
        Assert.Contains(resp.Profiles, profile => profile.Name == "User");
    }
    
    // Sadly mounting the docker socket wont allow the wolf container to run anymore, reason still unkown
    // TODO: Fix mounting docker socket allowing for tests that rely on docker access...
    //[Fact]
    //public async Task TestDockerInspect()
    //{
    //     var response = await Api.GetDockerImagesInspectAsync("ghcr.io/games-on-whales/wolf:stable");
    //     testOutputHelper.WriteLine(response.ToString());
    //}

    public Task InitializeAsync()
    {
        var tmpFolder = Path.GetTempPath();
        var path = Path.Join(tmpFolder, "GamesOnWhales.Wolf.Net.Bindings.Tests");
        
        Api = new WolfApi(new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                {"SOCKET_PATH", Path.Join(path, "cfg/wolf.sock")}
            }!).Build());
        
        return Task.CompletedTask;
    }
    
    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}