namespace Markwardt;

public static class BinaryExtensions
{
    public static T SetBit<T>(this T target, int bit)
        where T : IShiftOperators<T, int, T>, IBitwiseOperators<T, T, T>, INumber<T>
        => target | (T.One << bit);

    public static T ResetBit<T>(this T target, int bit)
        where T : IShiftOperators<T, int, T>, IBitwiseOperators<T, T, T>, INumber<T>
        => target & ~(T.One << bit);

    public static T FlipBit<T>(this T target, int bit)
        where T : IShiftOperators<T, int, T>, IBitwiseOperators<T, T, T>, INumber<T>
        => target ^ (T.One << bit);
}