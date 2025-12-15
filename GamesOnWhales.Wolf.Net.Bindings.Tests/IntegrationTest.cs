using System.Collections.ObjectModel;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace GamesOnWhales.Wolf.Net.Bindings.Tests;

public class ContainerFixture : IAsyncLifetime
{
    private IContainer TestContainer { get; set; } = null!;
    public async Task InitializeAsync()
    {
        var tmpFolder = Path.GetTempPath();
        var path = Path.Join(tmpFolder, "GamesOnWhales.Wolf.Net.Bindings.Tests");
        
        if(Directory.Exists(Path.Join(path, "cfg")))
            Directory.Delete(Path.Join(path, "cfg"), true);
            
        Directory.CreateDirectory(path);
        
        // Create a new instance of a container.
        TestContainer = new ContainerBuilder()
            .WithImage("ghcr.io/games-on-whales/wolf:stable")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged(".*HTTPS server listening on port.*"))
            .WithEnvironment(new Dictionary<string, string>
            {
                {"WOLF_SOCKET_PATH","/etc/wolf/cfg/wolf.sock"},
                {"WOLF_LOG_LEVEL", "DEBUG"}
            })
            .WithBindMount(path, "/etc/wolf")
            //.WithBindMount("/var/run/docker.sock", "/var/run/docker.sock")
            // Build the container configuration.
            .Build();
        
        await TestContainer.StartAsync();
        await TestContainer.ExecAsync(["chown", "-R", "1000:1000", "/etc/wolf"]);
    }
    
    public async Task DisposeAsync()
    {
        await TestContainer.StopAsync();
    }
}
public class IntegrationTest(ITestOutputHelper testOutputHelper, ContainerFixture containerFixture) : IAsyncLifetime, IClassFixture<ContainerFixture>
{
    //private ContainerFixture TestContainerFixture { get; set; } = null!;
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
    // [Fact]
    // public async Task TestDockerInspect()
    // {
    //     var response = await Api.GetDockerImagesInspectAsync("ghcr.io/games-on-whales/wolf:stable");
    //     testOutputHelper.WriteLine(response.ToString());
    // }

    public async Task InitializeAsync()
    {
        var tmpFolder = Path.GetTempPath();
        var path = Path.Join(tmpFolder, "GamesOnWhales.Wolf.Net.Bindings.Tests");
        
        Api = new WolfApi(new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                {"SOCKET_PATH", Path.Join(path, "cfg/wolf.sock")}
            }!).Build());
    }
    
    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}