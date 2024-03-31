namespace Markwardt;

[Singleton<TopLogger>]
public interface ITopLogger
{
    ILogHandler? Handler { get; set; }
}

public class TopLogger : Logger, ITopLogger, ILogHandler
{
    public required IConsole Console { get; init; }

    public required OutputLoggerFactory OutputLoggerFactory { get; init; }

    [Inject<GlobalLoggersTag>]
    public required IReadOnlyList<object> GlobalLoggers { get; init; }

    private Task<object>? defaultLoggerFactory;

    private ILogHandler? handler;
    public ILogHandler? Handler { get => handler; set => this.RaiseAndSetIfChanged(ref handler, value); }

    Maybe<LogMessage> ILogHandler.HandleLog(LogMessage message)
    {
        message = message.RemoveSource(this);
        return Handler is null ? message : Handler.HandleLog(message);
    }

    protected override void OnLog(LogMessage message)
    {
        DefaultLog(message);
        GlobalLoggers.ForEach(x => x.Log(message));
    }

    private async void DefaultLog(LogMessage message)
    {
        if (defaultLoggerFactory is null)
        {
            defaultLoggerFactory = CreateDefaultLogger().AsTask();
        }

        (await defaultLoggerFactory).Log(message);
    }

    private async ValueTask<object> CreateDefaultLogger()
        => await OutputLoggerFactory(Console);
}