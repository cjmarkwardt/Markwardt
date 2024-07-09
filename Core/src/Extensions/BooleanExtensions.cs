namespace Markwardt;

public static class BooleanExtensions
{
    public static byte ToByte(this bool value)
        => value ? (byte)1 : (byte)0;
}