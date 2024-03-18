namespace Markwardt;

public record LogMessage(string Sender, string Category, object? Content, string? Source = null)
{
    public static LogMessage FromCaller(string Sender, string Category, object? Content, [CallerFilePath] string sourcePath = "", [CallerLineNumber] int sourceLine = 0)
        => new(Sender, Category, Content, $"{sourcePath}:{sourceLine}");

    public LogMessage ShortenSource()
        => this with { Source = Source?.Transform(x => x.Split('/', '\\').Last()) };

    public override string ToString()
        => $"{Category} | {Content}{Source.Transform(x => $" ({x})")}";

    public string ToString(DateTime timestamp)
        => $"{timestamp} | {this}";
}