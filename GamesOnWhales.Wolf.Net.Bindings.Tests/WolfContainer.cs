using System.Diagnostics;
using System.Reflection;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Logging.Testing;


namespace GamesOnWhales.Wolf.Net.Bindings.Tests;

public class WolfContainer : IAsyncLifetime
{
    public IContainer TestContainer { get; private set; } = null!;
    public FakeLogCollector? FakeLogCollector { get; private set; }
    public Action<string>? LogAction { get; set; }
    
    public async Task InitializeAsync()
    {
        FakeLogCollector = FakeLogCollector.Create(new FakeLogCollectorOptions
        {
            OutputSink = message => LogAction?.Invoke(message)
        });

        var logger = new FakeLogger<WolfContainer>(FakeLogCollector);
        
        var tmpFolder = Path.GetTempPath();
        var path = Path.Join(tmpFolder, "GamesOnWhales.Wolf.Net.Bindings.Tests");
        
        if(!Directory.Exists(Path.Join(path, "cfg")))
            Directory.CreateDirectory(path);
        
        var ids = Utils.GetUserIDs(Environment.UserName);
        
        var assembly = typeof(WolfContainer).GetTypeInfo().Assembly;
        var resource = assembly.GetManifestResourceStream("GamesOnWhales.Wolf.Net.Bindings.Tests.test.png");

        if (resource == null)
        {
            throw new Exception("Resource not found");
        }
        
        // ReSharper disable once RedundantAssignment
        var fileBuffer = Array.Empty<byte>();
        var ms = new MemoryStream();
        await resource.CopyToAsync(ms);
        fileBuffer = ms.ToArray();
        
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
            .WithLogger(logger)
            .WithBindMount("/var/run/docker.sock", "/var/run/docker.sock", AccessMode.ReadWrite)
            //Todo: add .WithOutputConsumer() for even cleaner Logs
            .WithResourceMapping(fileBuffer , "/etc/wolf/test.png", uint.Parse(ids.uid), uint.Parse(ids.gid))
            
            // Build the container configuration.
            .Build();
        
        await TestContainer.StartAsync();
    }
    
    public async Task DisposeAsync()
    {
        await TestContainer.StopAsync();
    }
}