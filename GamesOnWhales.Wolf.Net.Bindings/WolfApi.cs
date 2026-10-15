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
    
    public WolfApi(ILogger<WolfApi>? logger = null,
        IConfiguration? configuration = null,
        IEnumerable<ISseEventHandler>? eventHandlers = null)
    {
        logger ??= NullLogger<WolfApi>.Instance;
        configuration ??= new ConfigurationBuilder().Build();
        
        _logger = logger;
        
        if (eventHandlers != null)
        {
            _sseHandlers = eventHandlers.ToDictionary(h => h.EventName, h => h);
        }
        _sseHandlers ??= new Dictionary<string, ISseEventHandler>();
        
        var socketPath = configuration.GetValue<string?>("SOCKET_PATH") ?? SocketPath;
        BaseUrl = configuration.GetValue<string?>("BASE_ADDRESS") ?? BaseUrl;
        var tokenLimit = configuration.GetValue<int?>("WOLF_API_TOKEN_LIMIT") ?? 10;
        var tokenPerInterval = configuration.GetValue<int?>("WOLF_API_TOKEN_PER_INTERVAL") ?? 10;
        
        var options = new TokenBucketRateLimiterOptions
        {
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            TokenLimit = tokenLimit,
            TokensPerPeriod = tokenPerInterval,
            QueueLimit = 1000, 
            ReplenishmentPeriod = TimeSpan.FromMilliseconds(100), 
            AutoReplenishment = true
        };
        
        if (socketPath.StartsWith("unix://"))
        {
            socketPath = socketPath["unix://".Length..];
            _httpClient = new HttpClient(
                handler: new ClientSideRateLimitedHandler(
                    limiter: new TokenBucketRateLimiter(options),
                    logger: _logger,
                    httpMessageHandler: new SocketsHttpHandler
                    {
                        ConnectCallback = async (_, token) =>
                        {
                            if (!Path.Exists(socketPath))
                            {
                                throw new FileNotFoundException(socketPath);
                            }

                            var retry = 0;
                            while (retry++ < 5)
                            {
                                try
                                {
                                    var socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.IP);
                                    var endpoint = new UnixDomainSocketEndPoint(socketPath);
                                    await socket.ConnectAsync(endpoint, token);
                                    return new NetworkStream(socket, ownsSocket: true);
                                }
                                catch (SocketException e)
                                {
                                    LogErrorWhileConnectingToSocketMsg(e.Message);
                                    await Task.Delay(10, token);
                                }
                            }
                            throw new IOException("Could not connect to socket");
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
                    logger: _logger,
                    httpMessageHandler: new HttpClientHandler()
                )
            );
        }
        
        GeneratedClient = new GeneratedApiClient(_httpClient)
        {
            BaseUrl = BaseUrl
        };

        ProfilesUpdated += (_, profiles) => OnProfilesUpdatedEvent(profiles);
    }

    [LoggerMessage(LogLevel.Warning, "Error while connecting to socket: {msg}")]
    partial void LogErrorWhileConnectingToSocketMsg(string msg);
}