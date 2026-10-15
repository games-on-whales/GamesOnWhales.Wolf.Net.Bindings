using Microsoft.Extensions.Logging;

namespace GamesOnWhales.Components;

using System.Globalization;
using System.Net;
using System.Threading.RateLimiting;

internal sealed class ClientSideRateLimitedHandler(RateLimiter limiter, ILogger logger, HttpMessageHandler httpMessageHandler)
    : DelegatingHandler(httpMessageHandler), IAsyncDisposable
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        using var lease = await limiter.AcquireAsync(permitCount: 1, cancellationToken);

        var retry = 0;
        while (retry++ < 5)
        {
            try
            {
                var response = await base.SendAsync(request, cancellationToken);
                return response;
            }
            catch (HttpRequestException e)
            {
                logger.LogWarning("Could not send request, retrying...");
            }
        }
        throw new InvalidOperationException("Could not Send the request");
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    { 
        await limiter.DisposeAsync().ConfigureAwait(false);

        Dispose(disposing: false);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            limiter.Dispose();
        }
    }
}