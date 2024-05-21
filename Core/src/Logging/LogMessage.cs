namespace Markwardt;

public record LogMessage(DateTime Timestamp, object Content, IEnumerable<string> Category, IEnumerable<object> Sources, LogLocation? Location = null)
{
    public static LogMessage FromCaller(object content, IEnumerable<string>? category = null, object? source = null, [CallerFilePath] string? path = null, [CallerLineNumber] int line = -1)
        => new(DateTime.Now, content, category ?? [], source is null ? [] : [source], path is null ? null : new LogLocation(path, ReadLine(line)));

    private static int? ReadLine(int line)
        => line < 0 ? null : line;

    private IEnumerable<string> SourceDisplayParts => Enumerable.Empty<string>()
        .When(_ => Sources.Any(), x => x.Concat(Sources.Select(x => x is ILogIdentifiable identifiable ? identifiable.GetLogIdentifier() ?? x.GetType().Name : x.GetType().Name).WhereNotNull()));

    private IEnumerable<string> ToStringParts => Enumerable.Empty<string>()
        .Append(TimestampDisplay)
        .When(_ => !string.IsNullOrWhiteSpace(CategoryDisplay), x => x.Append(CategoryDisplay))
        .When(_ => !string.IsNullOrWhiteSpace(SourceDisplay), x => x.Append(SourceDisplay))
        .Append(ContentDisplay);

    public string TimestampDisplay => Timestamp.ToString("d-MMM-y h:mm:ss");
    public string CategoryDisplay => string.Join("/", Category);
    public string SourceDisplay => string.Join(" -> ", SourceDisplayParts);
    public string ContentDisplay => Content.ToString() ?? "<null>";

    public bool IsFailure => Content is Exception;

    public LogMessage AddSource(object source)
        => this with { Sources = Sources.Append(source) };

    public LogMessage RemoveSource(object source)
        => this with { Sources = Sources.Where(x => x != source) };

    public override string ToString()
        => string.Join(" | ", ToStringParts);
}