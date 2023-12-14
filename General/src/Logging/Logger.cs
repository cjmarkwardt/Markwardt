namespace Markwardt;

public interface ILogger
{
    IObservable<LogReport> LogReported { get; }

    void Log(LogReport report);
}

public static class LoggerExtensions
{
    public static void Log(this ILogger logger, string category, string message, [CallerFilePath] string sourcePath = "", [CallerLineNumber] int sourceLine = 0)
        => logger.Log(new LogReport(category, message, sourcePath, sourceLine));

    public static IDisposable RouteLogsTo(this ILogger source, ILogger destination)
        => source.LogReported.Subscribe(x => destination.Log(x));
}