using System.Net.ServerSentEvents;
using Microsoft.Extensions.Hosting;

namespace GamesOnWhales;
using Microsoft.Extensions.Logging;

public partial class WolfApi : IHostedService
{
    private CancellationTokenSource? _sseCancellationTokenSource;
    private Task? _sseListeningTask;
    
    private async Task ListenAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var stream = await _httpClient.GetStreamAsync($"{BaseUrl}api/v1/events", stoppingToken);
                var eventType = string.Empty;

                await foreach (var item in SseParser.Create(stream).EnumerateAsync(stoppingToken))
                {
                    if (item.Data == ":keepalive") continue;

                    if (item.Data.StartsWith("event:"))
                        eventType = item.Data["event: ".Length..];

                    if (!item.Data.StartsWith("data:")) continue;

                    await FilterApiEvents(eventType, item.Data["data: ".Length..]);
                }

                _logger.LogError("Lost connection to the Wolf API SSE. End of Stream.");
                await Emit(SseConnectionLostEvent, false);
                await OnSseConnectionLostEvent(false);
                await Task.Delay(1000, stoppingToken);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(
                    "The Wolf API SSE encountered an HttpRequestException exception: Statuscode: {statuscode} - {message}",
                    e.StatusCode.ToString(),
                    e.Message);
                await Emit(SseConnectionLostEvent, true);
                await OnSseConnectionLostEvent(true);
                await Task.Delay(5000, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation("SSE read stopped");
                return;
            }
        }
    }
    
    private async Task FilterApiEvents(string @event, string data)
    {
        if(_logger.IsEnabled(LogLevel.Debug))
            _logger.LogDebug("Event {event}", @event);
    
        if (!_sseHandlers.TryGetValue(@event, out var handler))
        {
            if(_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug("no EventHandler for: {event} registered.", @event);
            
            await Emit(SseEvent, (@event, data));
            await OnEvent(@event, data);
            return;
        }

        await handler.Call(this, data);
    }

    public event Func<object, bool, Task>? SseConnectionLostEvent;
    protected virtual Task OnSseConnectionLostEvent(bool isFatal) => Task.CompletedTask;
    
    /// <summary>
    /// Gets Invoked if no EventHandler is registered for the received SSE event.
    /// </summary>
    public event Func<object, (string @event, string data), Task>? SseEvent;
    
    /// <summary>
    /// <c>OnEvent</c> gets called if no EventHandler is registered for <c>@event</c>.
    /// </summary>
    /// <param name="event">the Identifying string for the SSE event.</param>
    /// <param name="data">the SSE events content in JSON format.</param>
    /// <returns></returns>
    protected virtual Task OnEvent(string @event, string data) => Task.CompletedTask;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (_sseListeningTask is { IsCompleted: false }) return Task.CompletedTask;
        
        _sseCancellationTokenSource ??= new CancellationTokenSource();
        _sseListeningTask = ListenAsync(_sseCancellationTokenSource.Token);
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _sseCancellationTokenSource?.Cancel();
        
        if (_sseListeningTask is { IsCompleted: false })
        {
            try
            {
                await _sseListeningTask;
            }
            catch (TaskCanceledException)
            { }
        }
        
        _sseCancellationTokenSource?.Dispose();
        _sseCancellationTokenSource = null;
        _sseListeningTask = null;
    }
}