using Microsoft.Extensions.Hosting;

namespace GamesOnWhales;
using Microsoft.Extensions.Logging;

public partial class WolfApi : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken) =>
        await BackgroundProcessing(stoppingToken);

    private async Task BackgroundProcessing(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                var stream = await _httpClient.GetStreamAsync($"{BaseUrl}api/v1/events", token);
                var eventType = string.Empty;
                using var reader = new StreamReader(stream);
                while (await reader.ReadLineAsync(token) is { } line)
                {
                    if(line == ":keepalive") continue;

                    if (line.StartsWith("event:"))
                        eventType = line["event: ".Length..];

                    if (!line.StartsWith("data:")) continue;
                    
                    await FilterApiEvents(eventType, line["data: ".Length..]);
                }

                _logger.LogError("Lost connection to the Wolf API SSE. End of Stream.");
                await Emit(SseConnectionLostEvent, false);
                await OnSseConnectionLostEvent(false);
                await Task.Delay(1000, token);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError("The Wolf API SSE encountered an HttpRequestException exception: Statuscode: {statuscode} - {message}", 
                    e.StatusCode.ToString(),
                    e.Message);
                await Emit(SseConnectionLostEvent, true);
                await OnSseConnectionLostEvent(true);
                await Task.Delay(5000, token);
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
    
    public event Func<object, (string @event, string data), Task>? SseEvent;
    protected virtual Task OnEvent(string @event, string data) => Task.CompletedTask;
}