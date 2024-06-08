namespace Markwardt;

public record struct EntityId(string Value)
{
    public override readonly string ToString()
        => Value;
}