namespace Markwardt;

public static class LoggingExtensions
{
    private static readonly ConditionalWeakTable<object, Subject<LogMessage>> subjects = [];

    public static IObservable<LogMessage> ObserveLogs(this object target)
        => subjects.GetOrCreateValue(target);

    public static void Log(this object target, LogMessage message)
    {
        LogMessage? handledMessage = message;
        if (target is not ILogHandler handler || handler.HandleLog(message).TryGetValue(out handledMessage))
        {
            subjects.GetOrCreateValue(target).OnNext(handledMessage);
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

    public static async ValueTask<IDisposable> RouteLogsToTop(this object target, IServiceResolver resolver)
        => target.RouteLogsTo(await resolver.RequireDefault<ITopLogger>());

    public static IDisposable Fork(this object target, Func<IDisposable, CancellationToken, ValueTask> action)
    {
        CancellationTokenSource cancellation = new();
        IDisposable cancel = Disposable.Create(cancellation.Cancel);

        async void Start()
        {
            try
            {
                await action(cancel, cancellation.Token);
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