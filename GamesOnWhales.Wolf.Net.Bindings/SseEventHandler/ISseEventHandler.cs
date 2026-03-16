namespace GamesOnWhales;

public interface ISseEventHandler
{
    public Task Call(WolfApi api, string eventData);
    string EventName { get; }
}

public interface ISseEventHandler<T> : ISseEventHandler
{
    public Task Convert(string eventData, out T result);
}