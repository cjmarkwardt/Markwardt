namespace Markwardt;

public readonly record struct Bounds2<T>(Vector2<T> Start, Vector2<T> Size)
    where T : INumber<T>
{
    public static Bounds2<T> FromEnd(Vector2<T> start, Vector2<T> end)
        => new(start, end - start);

    public Vector2<T> End => Start + Size;
}