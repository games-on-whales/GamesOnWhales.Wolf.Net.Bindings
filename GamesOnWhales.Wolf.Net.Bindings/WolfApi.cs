namespace GamesOnWhales;

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using System.Threading.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Components;

public partial class WolfApi : IHostedService, IWolfApi
{
    public NSwagDocker.NSwagDocker GeneratedDockerApiBindings { get; }
    public NSwagWolfApi.NSwagWolfApi GeneratedApiBindings { get; }
    public GeneratedApiClient GeneratedApiClient { get; }
    private string SocketPath { get; set; } = "/etc/wolf/cfg/wolf.sock";
    private string BaseUrl { get; set; } = "http://localhost/";
    
    private readonly ILogger<WolfApi> _logger;
    private readonly HttpClient _httpClient;
    
    public ConcurrentQueue<NSwagWolfApi.Profile>? Profiles { get; private set; }

    public WolfApi() : this(NullLogger<WolfApi>.Instance) { }
    
    public WolfApi(ILogger<WolfApi> logger) : 
        this(logger, 
            new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                {"SOCKET_PATH", "/etc/wolf/cfg/wolf.sock"}
            }!).Build())
    { }
    
    public WolfApi(IConfiguration config) : this(NullLogger<WolfApi>.Instance, config) { }
    
    public WolfApi(ILogger<WolfApi> logger, IConfiguration configuration)
    {
        var socketPath = configuration.GetValue<string?>("SOCKET_PATH") ?? SocketPath;
        BaseUrl = configuration.GetValue<string?>("BASE_ADDRESS") ?? BaseUrl;
        
        var options = new TokenBucketRateLimiterOptions
        { 
            TokenLimit = 8, 
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 3, 
            ReplenishmentPeriod = TimeSpan.FromMilliseconds(1), 
            TokensPerPeriod = 2, 
            AutoReplenishment = true
        };
        
        if (!string.IsNullOrEmpty(socketPath))
        {
            _httpClient = new HttpClient(
                handler: new ClientSideRateLimitedHandler(
                    limiter: new TokenBucketRateLimiter(options),
                    httpMessageHandler: new SocketsHttpHandler
                    {
                        ConnectCallback = async (_, token) =>
                        {
                            if (!Path.Exists(socketPath))
                            {
                                throw new FileNotFoundException(socketPath);
                            }

                            var socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.IP);
                            var endpoint = new UnixDomainSocketEndPoint(socketPath);
                            await socket.ConnectAsync(endpoint, token);
                            return new NetworkStream(socket, ownsSocket: true);
                        }
                    }
                )
            );
        }
        else
        {
            _httpClient = new HttpClient(
                handler: new ClientSideRateLimitedHandler(
                    limiter: new TokenBucketRateLimiter(options),
                    httpMessageHandler: new HttpClientHandler()
                )
            );
        }
        
        GeneratedDockerApiBindings = new NSwagDocker.NSwagDocker(_httpClient)
        {
            BaseUrl = BaseUrl
        };
        GeneratedApiBindings = new NSwagWolfApi.NSwagWolfApi(_httpClient)
        {
            BaseUrl = BaseUrl
        };
        GeneratedApiClient = new GeneratedApiClient(_httpClient)
        {
            BaseUrl = BaseUrl
        };
        
        _logger = logger;
    }
    
    [MemberNotNull(nameof(Profiles))]
    public async Task UpdateProfiles()
    {
        Profiles = new ConcurrentQueue<NSwagWolfApi.Profile>();
        var profiles = await GetProfiles();
        foreach (var profile in profiles)
        {
            Profiles.Enqueue(profile);
        }
        await OnProfilesUpdatedEvent(profiles);
    }
    
    public async Task<NSwagWolfApi.GenericSuccessResponse> AddProfile(NSwagWolfApi.Profile profile)
    {
        var val = await GeneratedApiBindings.Add2Async(profile);
        await UpdateProfiles();
        return val;
    }

    public async Task<NSwagWolfApi.GenericSuccessResponse> DeleteProfile(NSwagWolfApi.Profile profile)
        => await DeleteProfile(profile.Id);

    public async Task<NSwagWolfApi.GenericSuccessResponse> DeleteProfile(string id)
    {
        var val = await GeneratedApiBindings.RemoveAsync(new NSwagWolfApi.ProfileRemoveRequest()
        {
            Id = id
        });
        await UpdateProfiles();
        return val;
    }

    public async Task<NSwagWolfApi.GenericSuccessResponse> UpdateProfile(string id, NSwagWolfApi.Profile profile)
    {
        if(id != profile.Id) throw new ArgumentException("id must be equal to profile id", nameof(id));
        var del = await GeneratedApiBindings.RemoveAsync(new NSwagWolfApi.ProfileRemoveRequest()
        {
            Id = id
        });
        if (!del.Success) return del;
        var add = await GeneratedApiBindings.Add2Async(profile);
        // Todo: If Delete Succeeds and Add Fails, Try rescue Profile by some means, or wait for a Dedicated Update endpoint.
        await UpdateProfiles();
        return add;
    }

    public async Task<ICollection<NSwagWolfApi.Profile>> GetProfiles() =>
        (await GeneratedApiBindings.ProfilesAsync()).Profiles ?? Array.Empty<NSwagWolfApi.Profile>();

    //public event IApiEventPublisher.ProfilesUpdatedEventHandler? ProfilesUpdatedEvent;

    protected virtual Task OnProfilesUpdatedEvent(ICollection<NSwagWolfApi.Profile> profiles)
    {
        return Task.CompletedTask;
    }
    
}