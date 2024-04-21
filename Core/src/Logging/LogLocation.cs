namespace Markwardt;

public record LogLocation(string Path, int? Line)
{
    public string ToString(bool fullPath)
        => (fullPath ? Path : System.IO.Path.GetFileName(Path)) + (Line is null ? string.Empty : $":{Line}");

    public override string ToString()
        => ToString(false);
}