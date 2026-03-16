namespace GamesOnWhales;

[SseEventHandler]
public partial class DockerPullImageEndEventHandler : ISseEventHandler<string>
{
    // Todo: Implement Convert to correct dto.
    public Task Convert(string eventData, out string result)
    {
        result = eventData;
        return Task.CompletedTask;
    }

    public string EventName => "DockerPullImageEndEvent";
}