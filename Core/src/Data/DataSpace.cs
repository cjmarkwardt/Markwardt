namespace Markwardt;

public interface IDataSpace
{
    ValueTask Load(long id, IDynamicBuffer destination);
    ValueTask Edit(Func<IDataSpaceEditor, ValueTask> edit);
}

public interface IDataSpaceEditor
{
    ValueTask<long> Create(ReadOnlyMemory<byte>? source = null);
    ValueTask Save(long id, ReadOnlyMemory<byte> source);
    ValueTask Delete(long id);
}

public static class DataSpaceExtensions
{
    public static async ValueTask LoadRoot(this IDataSpace space, IDynamicBuffer destination)
        => await space.Load(0, destination);

    public static async ValueTask SaveRoot(this IDataSpaceEditor space, ReadOnlyMemory<byte> source)
        => await space.Save(0, source);
}

public class DataSpace : IDataSpace
{
    public DataSpace(Stream stream, int blockSize)
    {
        this.stream = stream;
        this.blockSize = blockSize;
        blockBuffer = new byte[blockSize];

        header = new(this);
        editor = new(this);
    }

    private readonly Stream stream;
    private readonly int blockSize;
    private readonly Header header;
    private readonly Editor editor;
    private readonly Memory<byte> blockBuffer;
    private readonly byte[] longBuffer = new byte[8];
    private readonly byte[] intBuffer = new byte[4];
    private readonly SequentialExecutor executor = new();

    private bool isHeaderRead;

    public async ValueTask Load(long id, IDynamicBuffer destination)
        => await executor.Execute(async () =>
        {
            long nextBlock = id;
            destination.Clear();
            do
            {
                MoveToBlock(nextBlock);
                await stream.ReadAsync(blockBuffer);

                nextBlock = BitConverter.ToInt64(blockBuffer.Span);

                Memory<byte> content;
                if (nextBlock == -1)
                {
                    ushort length = BitConverter.ToUInt16(blockBuffer[8..].Span);
                    content = blockBuffer.Slice(10, length);
                }
                else
                {
                    content = blockBuffer[8..];
                }

                destination.AppendFrom(content.Span);
            }
            while (nextBlock != -1);
        });

    public async ValueTask Edit(Func<IDataSpaceEditor, ValueTask> edit)
        => await executor.Execute(async () =>
        {
            if (!isHeaderRead)
            {
                isHeaderRead = true;
                await header.Read();
            }

            await edit(editor);
            await header.Write();
        });
    
    private long GetBlockOffset(long index)
        => Header.Size + (index * blockSize);

    private void MoveToBlock(long index)
        => stream.Position = GetBlockOffset(index);

    private async ValueTask<long> ReadNextBlock(long index)
    {
        long originalPosition = stream.Position;
        stream.Position = GetBlockOffset(index);
        await stream.ReadAsync(longBuffer);
        stream.Position = originalPosition;
        return BitConverter.ToInt64(longBuffer);
    }

    private async ValueTask WriteNextBlock(long index, long value)
    {
        long originalPosition = stream.Position;
        stream.Position = GetBlockOffset(index);
        BitConverter.TryWriteBytes(longBuffer, value);
        await stream.WriteAsync(longBuffer);
        stream.Position = originalPosition;
    }

    private async ValueTask WriteLength(long index, long value)
    {
        long originalPosition = stream.Position;
        stream.Position = GetBlockOffset(index);
        BitConverter.TryWriteBytes(intBuffer, value);
        await stream.WriteAsync(intBuffer);
        stream.Position = originalPosition;
    }

    private sealed class Header(DataSpace space)
    {
        public static int Size => 8 + 8 + 8;

        private readonly Memory<byte> buffer = new byte[Size];

        public long NewBlock { get; set; }
        public long FirstFreeBlock { get; set; }
        public long LastFreeBlock { get; set; }

        public async ValueTask Read()
        {
            MoveToHeader();
            await space.stream.ReadAsync(buffer);
            NewBlock = BitConverter.ToInt64(buffer.Span);
            FirstFreeBlock = BitConverter.ToInt64(buffer[8..].Span);
            LastFreeBlock = BitConverter.ToInt64(buffer[16..].Span);
        }

        public async ValueTask Write()
        {
            BitConverter.TryWriteBytes(buffer.Span, NewBlock);
            BitConverter.TryWriteBytes(buffer[8..].Span, FirstFreeBlock);
            BitConverter.TryWriteBytes(buffer[16..].Span, LastFreeBlock);
            MoveToHeader();
            await space.stream.WriteAsync(buffer);
        }

        private void MoveToHeader()
            => space.stream.Position = 0;
    }

    private sealed class Editor(DataSpace space) : IDataSpaceEditor
    {
        private readonly SequentialExecutor executor = new();

        public async ValueTask<long> Create(ReadOnlyMemory<byte>? source = null)
            => await executor.Execute(async () =>
            {
                long id = await Allocate();
                if (source is null)
                {
                    await space.WriteLength(id, 0);
                }
                else
                {
                    await WriteBlocks(id, source.Value);
                }

                return id;
            });

        public async ValueTask Save(long id, ReadOnlyMemory<byte> source)
            => await executor.Execute(async () => await WriteBlocks(id, source));

        public async ValueTask Delete(long id)
            => await executor.Execute(async () => await Free(id));

        private async ValueTask WriteBlocks(long id, ReadOnlyMemory<byte> source)
        {
            int max = space.blockSize - 12;
            long nextBlock = id;
            while (true)
            {
                if (source.Length <= max)
                {
                    BitConverter.TryWriteBytes(space.blockBuffer.Span, (long)-1);
                    BitConverter.TryWriteBytes(space.blockBuffer[8..].Span, (ushort)source.Length);
                    source.CopyTo(space.blockBuffer[10..]);
                }
                else
                {

                }

                space.MoveToBlock(nextBlock);
                await space.stream.WriteAsync(space.blockBuffer);
            }
        }

        private async ValueTask<long> Allocate()
        {
            long index;
            if (space.header.FirstFreeBlock != -1)
            {
                index = space.header.FirstFreeBlock;

                if (space.header.LastFreeBlock == index)
                {
                    space.header.FirstFreeBlock = -1;
                    space.header.LastFreeBlock = -1;
                }
                else
                {
                    space.header.FirstFreeBlock = await space.ReadNextBlock(index);
                }
            }
            else
            {
                index = space.header.NewBlock;
                space.header.NewBlock++;
            }

            return index;
        }

        private async ValueTask Free(long index)
        {
            if (space.header.LastFreeBlock != -1)
            {
                await space.WriteNextBlock(space.header.LastFreeBlock, index);
            }
            
            space.header.LastFreeBlock = index;
        }
    }
}