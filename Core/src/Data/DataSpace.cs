namespace Markwardt;

public interface IDataSpace
{
    ValueTask Load(int id, IDynamicBuffer destination);
    ValueTask<int> Create(ReadOnlyMemory<byte>? source = null);
    ValueTask Save(int id, ReadOnlyMemory<byte> source);
    ValueTask Delete(int id);
}

public static class DataSpaceExtensions
{
    public static async ValueTask LoadRoot(this IDataSpace space, IDynamicBuffer destination)
        => await space.Load(0, destination);

    public static async ValueTask SaveRoot(this IDataSpace space, ReadOnlyMemory<byte> source)
        => await space.Save(0, source);
}

public class DataSpace : IDataSpace, IDataSpaceWriter
{
    private static int HeaderSize => sizeof(int) * 3;

    public DataSpace(Stream stream, int blockSize)
    {
        this.stream = stream;
        this.blockSize = blockSize;

        header = new(this);
        block = new(this, false);

        if (block.MaxContentSize < 1)
        {
            throw new InvalidOperationException($"Block size of {blockSize} is too small");
        }
    }

    private readonly Stream stream;
    private readonly int blockSize;
    private readonly DataHeaderWriter header;
    private readonly DataBlockWriter block;
    private readonly SequentialExecutor executor = new();

    bool IDataSpaceWriter.IsInitialized => stream.Length >= HeaderSize;
    int IDataSpaceWriter.HeaderSize => HeaderSize;
    int IDataSpaceWriter.BlockSize => blockSize;

    public async ValueTask Load(int id, IDynamicBuffer destination)
        => await executor.Execute(async () =>
        {
            destination.Clear();

            int index = id;
            while (index != -1)
            {
                await block.Read(index);
                destination.AppendFrom(block.Content.Span);
                index = block.NextBlock;
            }
        });

    public async ValueTask<int> Create(ReadOnlyMemory<byte>? source = null)
        => await executor.Execute(async () =>
        {
            int id = await header.Allocate();
            await SetData(id, source);
            await header.Write();
            return id;
        });

    public async ValueTask Save(int id, ReadOnlyMemory<byte> source)
        => await executor.Execute(async () =>
        {
            await SetData(id, source);
            await header.Write();
        });

    public async ValueTask Delete(int id)
        => await executor.Execute(async () =>
        {
            await header.Free(id);
            await header.Write();
        });
    
    private long GetBlockOffset(int index)
        => HeaderSize + ((long)index * blockSize);

    private async ValueTask SetData(int id, ReadOnlyMemory<byte>? source)
    {
        source ??= Array.Empty<byte>();

        int index = id;
        while (index != -1)
        {
            int nextBlock = await header.ReadNextBlock(index);

            if (source.Value.Length <= block.MaxContentSize)
            {
                if (nextBlock != -1)
                {
                    await header.Free(nextBlock);
                }

                block.SetFinal(source.Value.Length);
                source.Value.CopyTo(block.Content);
                await block.Write(index);
                break;
            }
            else
            {
                if (nextBlock == -1)
                {
                    nextBlock = await header.Allocate();
                }

                block.SetContinuation(nextBlock);
                source.Value[..block.MaxContentSize].CopyTo(block.Content);
                await block.Write(index);
                source = source.Value[block.MaxContentSize..];
                index = nextBlock;
            }
        }
    }

    async ValueTask IDataSpaceWriter.ReadHeader(Memory<byte> destination)
    {
        stream.Position = 0;
        await stream.ReadAsync(destination);
    }

    async ValueTask IDataSpaceWriter.WriteHeader(ReadOnlyMemory<byte> source)
    {
        stream.Position = 0;
        await stream.WriteAsync(source);
    }

    async ValueTask IDataSpaceWriter.ReadBlock(int index, Memory<byte> destination)
    {
        stream.Position = GetBlockOffset(index);
        await stream.ReadAsync(destination);
    }

    async ValueTask IDataSpaceWriter.WriteBlock(int index, ReadOnlyMemory<byte> source)
    {
        stream.Position = GetBlockOffset(index);
        await stream.WriteAsync(source);
    }
}