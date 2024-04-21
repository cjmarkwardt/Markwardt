namespace Markwardt;

public readonly record struct Vector3<T>(T X, T Y, T Z)
    where T : INumber<T>
{
    public static Vector3<T> operator +(Vector3<T> a, Vector3<T> b)
        => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        
    public static Vector3<T> operator -(Vector3<T> a, Vector3<T> b)
        => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        
    public static Vector3<T> operator *(Vector3<T> a, Vector3<T> b)
        => new(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        
    public static Vector3<T> operator /(Vector3<T> a, Vector3<T> b)
        => new(a.X / b.X, a.Y / b.Y, a.Z / b.Z);

    public static Vector3<T> operator +(Vector3<T> a, T b)
        => new(a.X + b, a.Y + b, a.Z + b);
        
    public static Vector3<T> operator -(Vector3<T> a, T b)
        => new(a.X - b, a.Y - b, a.Z - b);
        
    public static Vector3<T> operator *(Vector3<T> a, T b)
        => new(a.X * b, a.Y * b, a.Z * b);
        
    public static Vector3<T> operator /(Vector3<T> a, T b)
        => new(a.X / b, a.Y / b, a.Z / b);

    public static Vector3<T> Zero => new(T.Zero);
    public static Vector3<T> One => new(T.One);

    public Vector3(T value)
        : this(value, value, value) { }
}