namespace Markwardt;

public interface ILogger : IMultiDisposable
{
    string LoggerName { get; }
    IObservable<LogMessage> LogReported { get; }

    void Log(LogMessage report);
}

public static class LoggerExtensions
{
    public static void Log(this ILogger logger, string category, object? content, [CallerFilePath] string sourcePath = "", [CallerLineNumber] int sourceLine = 0)
        => logger.Log(LogMessage.FromCaller(logger.LoggerName, category, content, sourcePath, sourceLine));

    public static void LogActivity(this ILogger logger, object? content, [CallerFilePath] string sourcePath = "", [CallerLineNumber] int sourceLine = 0)
        => logger.Log("Activity", content, sourcePath, sourceLine);

    public static void LogError(this ILogger logger, object? content, [CallerFilePath] string sourcePath = "", [CallerLineNumber] int sourceLine = 0)
        => logger.Log("Error", content, sourcePath, sourceLine);

    public static void LogError(this ILogger logger, Failable failable, [CallerFilePath] string sourcePath = "", [CallerLineNumber] int sourceLine = 0)
    {
        if (failable.Exception != null)
        {
            logger.LogError(failable.Exception, sourcePath, sourceLine);
        }
    }

    public static IDisposable RouteLogsTo(this ILogger logger, ILogger destination, Func<LogMessage, LogMessage>? transform = null)
        => logger.LogReported.Subscribe(x => destination.Log((transform?.Invoke(x) ?? x) with { Sender = $"{destination.LoggerName}/{logger.LoggerName}" }));
}