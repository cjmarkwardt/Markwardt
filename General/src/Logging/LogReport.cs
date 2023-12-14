namespace Markwardt;

public record LogReport(string Category, string Message, string? Source = null)
{
    public static LogReport FromCaller(string Category, string Message, [CallerFilePath] string sourcePath = "", [CallerLineNumber] int sourceLine = 0)
        => new(Category, Message, $"{sourcePath}:{sourceLine}");
}