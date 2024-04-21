namespace Markwardt;

public abstract class Logger : ReactiveObject
{
    protected Logger()
        => this.ObserveLogs().Subscribe(OnLog);

    protected abstract void OnLog(LogMessage message);
}