namespace Markwardt;

public interface ILogHandler
{
    Maybe<LogMessage> HandleLog(LogMessage message);
}