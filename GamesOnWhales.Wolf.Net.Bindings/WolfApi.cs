using GamesOnWhales.SSE;

namespace GamesOnWhales;

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using System.Threading.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Components;

public partial class WolfApi : IWolfApi
{
    public GeneratedApiClient GeneratedClient { get; }
    private string SocketPath { get; set; } = "unix:///etc/wolf/cfg/wolf.sock";
    private string BaseUrl { get; set; } = "http://localhost/";
    
    private readonly ILogger<WolfApi> _logger;
    private readonly HttpClient _httpClient;
    
    private readonly Dictionary<string, ISseEventHandler> _sseHandlers;
    
    public WolfApi(ILogger<WolfApi>? logger = null, IConfiguration? configuration = null, IEnumerable<ISseEventHandler>? eventHandlers = null)
    {
        logger ??= NullLogger<WolfApi>.Instance;
        configuration ??= new ConfigurationBuilder().Build();
        
        if (eventHandlers != null)
        {
            _sseHandlers = eventHandlers.ToDictionary(h => h.EventName, h => h);
        }
        _sseHandlers ??= new Dictionary<string, ISseEventHandler>();
        
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
        
        if (socketPath.StartsWith("unix://"))
        {
            socketPath = socketPath["unix://".Length..];
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
        
        GeneratedClient = new GeneratedApiClient(_httpClient)
        {
            BaseUrl = BaseUrl
        };
        
        _logger = logger;

        ProfilesUpdated += (_, profiles) => OnProfilesUpdatedEvent(profiles);
    }
}