namespace Markwardt;

[Singleton<Logger>]
public interface ILogger : ILoggable, ILogReceiver;

public class Logger : ILogger
{
    private readonly Subject<LogMessage> logged = new();
    public IObservable<LogMessage> Logged => logged;

    [Inject<LoggablesTag>]
    public required IReadOnlyList<ILoggable> Loggables { get; init; }

    [Inject<DisabledLogCategoriesTag>]
    public required IReadOnlyList<IEnumerable<string>> DisabledCategories { get; init; }

    public void Log(LogMessage message)
    {
        if (DisabledCategories.Any(x => x.MinSequenceEqual(message.Category)))
        {
            return;
        }

        message = message.RemoveSource(this);
        Loggables.ForEach(x => x.Log(message));
        logged.OnNext(message);
    }
}