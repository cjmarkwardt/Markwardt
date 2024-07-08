namespace Markwardt;

public static class LazyDataPointerExtensions
{
    public static IDataPointer LazyMove(this IDataPointer data, int offset, Func<ValueTask<IDataPointer>> create)
        => new LazyPointer(data.Position + offset, create);
}

public class LazyPointer(int position, Func<ValueTask<IDataPointer>> create) : IDataPointer
{
    private readonly AsyncLazy<IDataPointer> data = new(create);

    public int Position => position;

    public IDataPointer Move(int offset)
        => data.TrySyncGet(out IDataPointer? value) ? value.Move(offset) : this.LazyMove(offset, async () => (await data.Get()).Move(offset));

    public async ValueTask<IDataPointer> Read(Memory<byte> destination, int length, int offset = 0, bool move = false)
        => await (await data.Get()).Read(destination, length, offset, move);

    public async ValueTask<IDataPointer> Write(ReadOnlyMemory<byte> source, int offset = 0, bool move = false)
        => await (await data.Get()).Write(source, offset, move);
}