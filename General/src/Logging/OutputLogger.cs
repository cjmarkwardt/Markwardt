namespace Markwardt;

[Factory<OutputLogger>]
public delegate ValueTask<object> OutputLoggerFactory(ITextOutput output, ILogFormatter? formatter = null);

public class OutputLogger(ITextOutput Output, ILogFormatter? Formatter = null) : Logger
{
    protected override void OnLog(LogMessage message)
        => Output.Write(Formatter?.Format(message) ?? message.ToString());
}