namespace Markwardt;

public record AssetId(string Module, string Value)
{
    public override string ToString()
        => $"{Module}:{Value}";
}