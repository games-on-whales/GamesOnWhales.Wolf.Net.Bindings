namespace GamesOnWhales;
using Microsoft.Extensions.Logging;

public partial class WolfApi
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(async () =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var stream = await _httpClient.GetStreamAsync($"{BaseUrl}api/v1/events", cancellationToken);
                    var eventType = string.Empty;
                    using var reader = new StreamReader(stream);
                    while (await reader.ReadLineAsync(cancellationToken) is { } line)
                    {
                        switch (line)
                        {
                            case ":keepalive":
                                continue;
                        }

                        if (line.StartsWith("event:"))
                            eventType = line["event: ".Length..];

                        if (!line.StartsWith("data:")) continue;
                        
                        var data = line["data: ".Length..];

                        await FilterApiEvents(eventType, data);
                    }

                    _logger.LogError("Lost connection to the Wolf API SSE. End of Stream.");
                    await OnSseConnectionLostEvent(false);
                    await Task.Delay(1000, cancellationToken);
                }
                catch (HttpRequestException e)
                {
                    _logger.LogError("The Wolf API SSE encountered an HttpRequestException exception: Statuscode: {statuscode} - {message}", 
                        e.StatusCode.ToString(),
                        e.Message);
                    await OnSseConnectionLostEvent(true);
                    await Task.Delay(5000, cancellationToken);
                }
            }
        }, CancellationToken.None);

        return Task.CompletedTask;
    }
    
    private async Task FilterApiEvents(string @event, string data)
    {
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

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public event Func<object, bool, Task>? SseConnectionLostEvent;
    protected virtual Task OnSseConnectionLostEvent(bool isFatal)
    {
        return Emit(SseConnectionLostEvent, isFatal);
    }
    
    public event Func<object, (string @event, string data), Task>? SseEvent;
    protected virtual Task OnEvent(string @event, string data)
    {
        return Emit(SseEvent, (@event, data));
    }
}