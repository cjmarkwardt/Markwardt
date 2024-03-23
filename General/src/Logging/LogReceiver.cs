namespace Markwardt;

[RoutedService<ILogger>]
public interface ILogReceiver
{
    IObservable<LogMessage> Logged { get; }
}

public static class LogReceiverExtensions
{
    public static IDisposable RouteLogsTo(this ILogReceiver receiver, ILoggable destination, object? target = null)
        => receiver.Logged.Subscribe(x => destination.Log(x.AddSource(target ?? destination)));
}