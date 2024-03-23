namespace Markwardt;

[RoutedService<ILogger>]
public interface ILoggable
{
    void Log(LogMessage message);
}

public static class LoggableExtensions
{
    public static void Log(this ILoggable loggable, object content, IEnumerable<string>? category = null, object? source = null, [CallerFilePath] string? path = null, [CallerLineNumber] int line = -1)
        => loggable.Log(LogMessage.FromCaller(content, category, source ?? loggable, path, line));

    public static void LogError(this ILoggable loggable, Exception content, IEnumerable<string>? category = null, object? source = null, [CallerFilePath] string? path = null, [CallerLineNumber] int line = -1)
        => loggable.Log(content, category, source, path, line);

    public static void LogError(this ILoggable loggable, string message, IEnumerable<string>? category = null, object? source = null, [CallerFilePath] string? path = null, [CallerLineNumber] int line = -1)
        => loggable.LogError(new InvalidOperationException(message), category, source, path, line);

    public static IDisposable Fork(this ILoggable loggable, AsyncAction<IDisposable> action, IEnumerable<string>? logCategory = null, object? logSource = null, [CallerFilePath] string? logLocationPath = null, [CallerLineNumber] int logLocationLine = -1)
    {
        CancellationTokenSource cancellation = new();
        IDisposable cancel = Disposable.Create(cancellation.Cancel);

        async void Start()
        {
            try
            {
                (await action(cancel, cancellation.Token)).WithLogging(loggable, null, null, logCategory, logSource, logLocationPath, logLocationLine);
            }
            catch (Exception exception)
            {
                if (exception is not OperationCanceledException)
                {
                    loggable.LogError(exception);
                }
            }
            finally
            {
                cancellation.Dispose();
            }
        }

        Start();

        return cancel;
    }
}