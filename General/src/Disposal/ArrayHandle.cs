namespace Markwardt;

public class ArrayHandle(int length) : Handle<nint>(Marshal.AllocHGlobal(length), x => Marshal.FreeHGlobal(x))
{
    public ArrayHandle(ReadOnlyMemory<byte> source)
        : this(source.Length)
    {
        if (!MemoryMarshal.TryGetArray(source, out ArraySegment<byte> segment))
        {
            segment = source.ToArray();
        }

        Marshal.Copy(segment.Array!, segment.Offset, Value, source.Length);
    }

    public int Length => length;
}