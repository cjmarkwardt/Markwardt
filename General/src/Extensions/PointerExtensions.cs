namespace Markwardt;

public static class PointerExtensions
{
    public static byte[] ToArray(this nint pointer, int size)
    {
        byte[] data = new byte[size];
        Marshal.Copy(pointer, data, 0, size);
        return data;
    }
}