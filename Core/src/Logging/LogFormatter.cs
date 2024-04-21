namespace Markwardt;

public interface ILogFormatter
{
    string Format(LogMessage message);
}