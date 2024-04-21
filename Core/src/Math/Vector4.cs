namespace Markwardt;

public readonly record struct Vector4<T>(T X, T Y, T Z, T W)
    where T : INumber<T>
{
    public static Vector4<T> operator +(Vector4<T> a, Vector4<T> b)
        => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
        
    public static Vector4<T> operator -(Vector4<T> a, Vector4<T> b)
        => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);
        
    public static Vector4<T> operator *(Vector4<T> a, Vector4<T> b)
        => new(a.X * b.X, a.Y * b.Y, a.Z * b.Z, a.W * b.W);
        
    public static Vector4<T> operator /(Vector4<T> a, Vector4<T> b)
        => new(a.X / b.X, a.Y / b.Y, a.Z / b.Z, a.W / b.W);

    public static Vector4<T> operator +(Vector4<T> a, T b)
        => new(a.X + b, a.Y + b, a.Z + b, a.W + b);
        
    public static Vector4<T> operator -(Vector4<T> a, T b)
        => new(a.X - b, a.Y - b, a.Z - b, a.W - b);
        
    public static Vector4<T> operator *(Vector4<T> a, T b)
        => new(a.X * b, a.Y * b, a.Z * b, a.W * b);
        
    public static Vector4<T> operator /(Vector4<T> a, T b)
        => new(a.X / b, a.Y / b, a.Z / b, a.W / b);

    public static Vector4<T> Zero => new(T.Zero);
    public static Vector4<T> One => new(T.One);

    public Vector4(T value)
        : this(value, value, value, value) { }
}