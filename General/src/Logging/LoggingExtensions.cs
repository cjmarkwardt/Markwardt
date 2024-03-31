namespace Markwardt;

public static class LoggingExtensions
{
    private static readonly ConditionalWeakTable<object, Subject<LogMessage>> subjects = [];

    public static IObservable<LogMessage> ObserveLogs(this object target)
        => subjects.GetOrCreateValue(target);

    public static void Log(this object target, LogMessage message)
    {
        if (target is not ILogHandler handler || handler.HandleLog(message).TryGetValue(out message))
        {
            subjects.GetOrCreateValue(target).OnNext(message);
        }
    }

    public static void Log(this object target, object content, IEnumerable<string>? category = null, object? source = null, [CallerFilePath] string? path = null, [CallerLineNumber] int line = -1)
        => target.Log(LogMessage.FromCaller(content, category, source ?? target, path, line));

    public static void LogError(this object target, Exception content, IEnumerable<string>? category = null, object? source = null, [CallerFilePath] string? path = null, [CallerLineNumber] int line = -1)
        => target.Log(content, category, source, path, line);

    public static void LogError(this object target, string message, IEnumerable<string>? category = null, object? source = null, [CallerFilePath] string? path = null, [CallerLineNumber] int line = -1)
        => target.LogError(new InvalidOperationException(message), category, source, path, line);

    public static IDisposable RouteLogsTo(this object target, object destination)
        => target.ObserveLogs().Subscribe(x => destination.Log(x.AddSource(destination)));

    public static IDisposable Fork(this object target, AsyncAction<IDisposable> action, IEnumerable<string>? logCategory = null, object? logSource = null, [CallerFilePath] string? logLocationPath = null, [CallerLineNumber] int logLocationLine = -1)
    {
        CancellationTokenSource cancellation = new();
        IDisposable cancel = Disposable.Create(cancellation.Cancel);

        async void Start()
        {
            try
            {
                (await action(cancel, cancellation.Token)).WithLogging(target, null, null, logCategory, logSource, logLocationPath, logLocationLine);
            }
            catch (Exception exception)
            {
                if (exception is not OperationCanceledException)
                {
                    target.LogError(exception);
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