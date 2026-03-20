using Microsoft.Extensions.Logging;

namespace GamesOnWhales.SSE;

public interface ISseEventHandler
{
    public Task Call(WolfApi api, string eventData);
    string EventName { get; }
}

internal interface ISseEventHandler<T> : ISseEventHandler
{
    public ILogger Logger { get; }
    public Task Convert(string eventData, out T result);
}