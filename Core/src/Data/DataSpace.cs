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

        header = new(async () => await DataSpaceHeader.Create(this));
        block = new(this, false);
    }

    private readonly Stream stream;
    private readonly int blockSize;
    private readonly AsyncLazy<DataSpaceHeader> header;
    private readonly DataBlock block;
    private readonly SequentialExecutor executor = new();

    int IDataSpaceWriter.HeaderSize => HeaderSize;
    int IDataSpaceWriter.BlockSize => blockSize;

    public async ValueTask Load(int id, IDynamicBuffer destination)
        => await executor.Execute(async () =>
        {
            int nextBlock = id;
            destination.Clear();
            do
            {
                MoveToBlock(nextBlock);
                await stream.ReadAsync(block);

                Memory<byte> content;
                if (nextBlock == -1)
                {
                    ushort length = BitConverter.ToUInt16(block[8..].Span);
                    content = block.Slice(10, length);
                }
                else
                {
                    content = block[8..];
                }

                destination.AppendFrom(content.Span);
            }
            while (nextBlock != -1);
        });

    public async ValueTask<int> Create(ReadOnlyMemory<byte>? source = null)
        => await executor.Execute(async () =>
        {
            await header.Read();
            int id = await header.Allocate();
            await header.Write();
            await SetData(id, source);
            return id;
        });

    public async ValueTask Save(int id, ReadOnlyMemory<byte> source)
        => await executor.Execute(async () => await SetData(id, source));

    public async ValueTask Delete(int id)
        => await executor.Execute(async () =>
        {
            await header.Read();
            await header.Free(id);
            await header.Write();
        });
    
    private long GetBlockOffset(int index)
        => HeaderSize + ((long)index * blockSize);

    private async ValueTask SetData(int id, ReadOnlyMemory<byte>? source)
    {
        if (source is null)
        {

            space.block.NextBlock = -1;
            space.block.ContentLength = 0;
            await space.block.Write(id, false);
        }
        else
        {
            int index = id;
            while (true)
            {
                if (source.Value.Length <= space.block.MaxContentLength)
                {
                    await space.block.Read(index, false);
                    if (space.block.NextBlock != -1)
                    {
                        await space.header.Free(space.block.NextBlock);
                    }

                    space.block.NextBlock = -1;
                    space.block.ContentLength = (ushort)source.Value.Length;
                    source.Value.CopyTo(space.block.Content);
                    await space.block.Write(index, true);
                    break;
                }
                else
                {
                    space.block.NextBlock = await space.header.Allocate();
                    space.block.ContentLength = 0;
                    source.Value[..space.block.MaxContentLength].CopyTo(space.block.Content);
                    source = source.Value[space.block.MaxContentLength..];
                }


                if (nextBlock is not null)
                {
                    index = nextBlock.Value;
                }
                else
                {
                    break;
                }
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