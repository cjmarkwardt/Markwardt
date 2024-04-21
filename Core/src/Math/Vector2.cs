namespace Markwardt;

public readonly record struct Vector2<T>(T X, T Y)
    where T : INumber<T>
{
    public static Vector2<T> operator +(Vector2<T> a, Vector2<T> b)
        => new(a.X + b.X, a.Y + b.Y);
        
    public static Vector2<T> operator -(Vector2<T> a, Vector2<T> b)
        => new(a.X - b.X, a.Y - b.Y);
        
    public static Vector2<T> operator *(Vector2<T> a, Vector2<T> b)
        => new(a.X * b.X, a.Y * b.Y);
        
    public static Vector2<T> operator /(Vector2<T> a, Vector2<T> b)
        => new(a.X / b.X, a.Y / b.Y);

    public static Vector2<T> operator +(Vector2<T> a, T b)
        => new(a.X + b, a.Y + b);
        
    public static Vector2<T> operator -(Vector2<T> a, T b)
        => new(a.X - b, a.Y - b);
        
    public static Vector2<T> operator *(Vector2<T> a, T b)
        => new(a.X * b, a.Y * b);
        
    public static Vector2<T> operator /(Vector2<T> a, T b)
        => new(a.X / b, a.Y / b);

    public static Vector2<T> Zero => new(T.Zero);
    public static Vector2<T> One => new(T.One);

    public Vector2(T value)
        : this(value, value) { }
}