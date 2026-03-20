namespace GamesOnWhales.Components;

using System.Globalization;
using System.Net;
using System.Threading.RateLimiting;

internal sealed class ClientSideRateLimitedHandler : DelegatingHandler, IAsyncDisposable
{
    private readonly RateLimiter _limiter;

    public ClientSideRateLimitedHandler(RateLimiter limiter, HttpMessageHandler httpMessageHandler) : base(httpMessageHandler)
    {
        _limiter = limiter;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        using RateLimitLease lease = await _limiter.AcquireAsync(
            permitCount: 1, cancellationToken);

        if (lease.IsAcquired)
        {
            return await base.SendAsync(request, cancellationToken);
        }

        var response = new HttpResponseMessage(HttpStatusCode.TooManyRequests);
        if (lease.TryGetMetadata(
                MetadataName.RetryAfter, out TimeSpan retryAfter))
        {
            response.Headers.Add(
                "Retry-After",
                ((int)retryAfter.TotalSeconds).ToString(
                    NumberFormatInfo.InvariantInfo));
        }

        return response;
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    { 
        await _limiter.DisposeAsync().ConfigureAwait(false);

        Dispose(disposing: false);
        GC.SuppressFinalize(this);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            _limiter.Dispose();
        }
    }
}