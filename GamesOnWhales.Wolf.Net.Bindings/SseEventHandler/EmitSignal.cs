namespace GamesOnWhales;

public partial class WolfApi
{
    private Task Emit<TEventArgs>(Func<object, TEventArgs, Task>? handlers, TEventArgs args)
    {
        if (handlers != null)
        {
            return Task.WhenAll(handlers.GetInvocationList()
                .OfType<Func<object, TEventArgs, Task>>()
                .Select(h => h(this, args)));
        }

        return Task.CompletedTask;
    }
}