namespace Markwardt;

public readonly record struct Bounds3<T>(Vector3<T> Start, Vector3<T> Size)
    where T : INumber<T>
{
    public static Bounds3<T> FromEnd(Vector3<T> start, Vector3<T> end)
        => new(start, end - start);

    public Vector3<T> End => Start + Size;
}