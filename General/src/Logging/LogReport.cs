namespace Markwardt;

public record LogReport(string Category, string Message, string? Source = null)
{
    public static LogReport FromCaller(string Category, string Message, [CallerFilePath] string sourcePath = "", [CallerLineNumber] int sourceLine = 0)
        => new(Category, Message, $"{sourcePath}:{sourceLine}");

    public LogReport ShortenSource()
        => this with { Source = Source?.Transform(x => x.Split('/', '\\').Last()) };

    public override string ToString()
        => $"{Category} | {Message}{Source.Transform(x => $" ({x})")}";

    public string ToString(DateTime timestamp)
        => $"{timestamp} | {this}";
}