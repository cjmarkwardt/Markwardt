namespace Markwardt;

public interface ILogger : IMultiDisposable
{
    IObservable<LogReport> LogReported { get; }

    void Log(LogReport report);
}

public static class LoggerExtensions
{
    public static void Log(this ILogger logger, string category, string message, [CallerFilePath] string sourcePath = "", [CallerLineNumber] int sourceLine = 0)
        => logger.Log(LogReport.FromCaller(category, message, sourcePath, sourceLine));

    public static void Error(this ILogger logger, string message, [CallerFilePath] string sourcePath = "", [CallerLineNumber] int sourceLine = 0)
        => logger.Log("Error", message, sourcePath, sourceLine);

    public static void Error(this ILogger logger, Exception exception, [CallerFilePath] string sourcePath = "", [CallerLineNumber] int sourceLine = 0)
        => logger.Error(exception.GetRecursiveMessage(), sourcePath, sourceLine);

    public static void Error(this ILogger logger, Failable failable, [CallerFilePath] string sourcePath = "", [CallerLineNumber] int sourceLine = 0)
    {
        if (failable.Exception != null)
        {
            logger.Error(failable.Exception, sourcePath, sourceLine);
        }
    }

    public static IDisposable RouteLogsTo(this ILogger source, ILogger destination)
        => source.LogReported.Subscribe(x => destination.Log(x));
}

public abstract class Logger : ManagedAsyncDisposable, ILogger
{
    private readonly Subject<LogReport> logReported = new();
    public IObservable<LogReport> LogReported => logReported;

    public void Log(LogReport report)
    {
        if (PushLog(report))
        {
            logReported.OnNext(report);
        }
    }

    protected abstract bool PushLog(LogReport report);
}