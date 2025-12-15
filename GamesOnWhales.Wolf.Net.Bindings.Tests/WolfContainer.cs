using System.Diagnostics;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;


namespace GamesOnWhales.Wolf.Net.Bindings.Tests;

// ReSharper disable once ClassNeverInstantiated.Global
public class WolfContainer : IAsyncLifetime
{
    private IContainer TestContainer { get; set; } = null!;
    
    public async Task InitializeAsync()
    {
        var tmpFolder = Path.GetTempPath();
        var path = Path.Join(tmpFolder, "GamesOnWhales.Wolf.Net.Bindings.Tests");
        
        if(!Directory.Exists(Path.Join(path, "cfg")))
            Directory.CreateDirectory(path);
        
        var ids = Utils.GetUserIDs(Environment.UserName);
        
        // Create a new instance of a container.
        TestContainer = new ContainerBuilder()
            .WithImage("ghcr.io/games-on-whales/wolf:stable")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged(".*HTTPS server listening on port.*"))
            .WithEnvironment(new Dictionary<string, string>
            {
                {"WOLF_SOCKET_PATH","/etc/wolf/cfg/wolf.sock"},
                {"WOLF_LOG_LEVEL", "DEBUG"},
                {"PUID", ids.uid},
                {"PGID", ids.gid},
                {"UNAME", Environment.UserName}
            })
            .WithBindMount(path, "/etc/wolf")
            //.WithBindMount("/var/run/docker.sock", "/var/run/docker.sock", AccessMode.ReadWrite)
            // Build the container configuration.
            .Build();
        
        await TestContainer.StartAsync();
    }
    
    public async Task DisposeAsync()
    {
        await TestContainer.StopAsync();
    }
}