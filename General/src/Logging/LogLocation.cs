namespace Markwardt;

public record LogLocation(string Path, int? Line)
{
    public override string ToString()
        => Path + (Line is null ? string.Empty : $":{Line}");
}