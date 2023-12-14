namespace Markwardt;

public record LogReport(string Category, string Message, string? Source = null)
{
    public LogReport(string Category, string Message, [CallerFilePath] string sourcePath = "", [CallerLineNumber] int sourceLine = 0)
        : this(Category, Message, $"{sourcePath}:{sourceLine}") { }
}