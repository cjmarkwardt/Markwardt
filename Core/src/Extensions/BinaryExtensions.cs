namespace Markwardt;

public static class BinaryExtensions
{
    public static T SetBit<T>(this T target, int bit)
        where T : IShiftOperators<T, int, T>, IBitwiseOperators<T, T, T>, INumber<T>
        => target | (T.One << bit);

    public static T SetBit<T>(this T target, int bit, bool value)
        where T : IShiftOperators<T, int, T>, IBitwiseOperators<T, T, T>, INumber<T>
        => value ? target.SetBit(bit) : target.ClearBit(bit);

    public static T ClearBit<T>(this T target, int bit)
        where T : IShiftOperators<T, int, T>, IBitwiseOperators<T, T, T>, INumber<T>
        => target & ~(T.One << bit);

    public static T FlipBit<T>(this T target, int bit)
        where T : IShiftOperators<T, int, T>, IBitwiseOperators<T, T, T>, INumber<T>
        => target ^ (T.One << bit);

    public static bool GetBit<T>(this T target, int bit)
        where T : IShiftOperators<T, int, T>, IBitwiseOperators<T, T, T>, INumber<T>
        => (target & (T.One << bit - 1)) != T.Zero;
}