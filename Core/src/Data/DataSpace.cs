namespace Markwardt;

public interface IDataSpace
{
    ValueTask Load(long id, IDynamicBuffer destination);
    ValueTask Edit(Func<IDataSpaceEditor, ValueTask> edit);
}

public interface IDataSpaceEditor
{
    ValueTask<long> Create();
    ValueTask Save(long id, ReadOnlyMemory<byte> source);
    ValueTask Delete(long id);
}

public static class DataSpaceExtensions
{
    public static async ValueTask LoadRoot(this IDataSpace space, IDynamicBuffer destination)
        => await space.Load(0, destination);

    public static async ValueTask SaveRoot(this IDataSpaceEditor space, ReadOnlyMemory<byte> source)
        => await space.Save(0, source);

    public static async ValueTask<long> Create(this IDataSpaceEditor space, ReadOnlyMemory<byte> source)
    {
        long id = await space.Create();
        await space.Save(id, source);
        return id;
    }
}

public class DataSpace : IDataSpace
{
    public DataSpace(Stream stream, int blockSize)
    {
        this.stream = stream;
        this.blockSize = blockSize;
        blockBuffer = new byte[blockSize];

        header = new(this);
        block = new(this);
        editor = new(this);
    }

    private readonly Stream stream;
    private readonly int blockSize;
    private readonly Header header;
    private readonly Block block;
    private readonly Editor editor;
    private readonly Memory<byte> blockBuffer;
    private readonly byte[] nextBlockBuffer = new byte[8];
    private readonly byte[] stopCountBuffer = new byte[4];
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
        await stream.ReadAsync(nextBlockBuffer);
        stream.Position = originalPosition;
        return BitConverter.ToInt64(nextBlockBuffer);
    }

    private async ValueTask WriteNextBlock(long index, long value)
    {
        long originalPosition = stream.Position;
        stream.Position = GetBlockOffset(index);
        BitConverter.TryWriteBytes(nextBlockBuffer, value);
        await stream.WriteAsync(nextBlockBuffer);
        stream.Position = originalPosition;
    }

    private async ValueTask WriteStopCount(long index, long value)
    {
        long originalPosition = stream.Position;
        stream.Position = GetBlockOffset(index);
        BitConverter.TryWriteBytes(stopCountBuffer, value);
        await stream.WriteAsync(stopCountBuffer);
        stream.Position = originalPosition;
    }

    private sealed class Header(DataSpace space)
    {
        public static int Size => 8 + 8 + 8;

        private readonly Memory<byte> data = new byte[Size];

        public long NewBlock
        {
            get => BitConverter.ToInt64(data.Span);
            set => BitConverter.TryWriteBytes(data.Span, value);
        }

        public long FirstFreeBlock
        {
            get => BitConverter.ToInt64(data[8..].Span);
            set => BitConverter.TryWriteBytes(data[8..].Span, value);
        }

        public long LastFreeBlock
        {
            get => BitConverter.ToInt64(data[16..].Span);
            set => BitConverter.TryWriteBytes(data[16..].Span, value);
        }

        public async ValueTask Read()
        {
            MoveToHeader();
            await space.stream.ReadAsync(data);
        }

        public async ValueTask Write()
        {
            MoveToHeader();
            await space.stream.WriteAsync(data);
        }

        private void MoveToHeader()
            => space.stream.Position = 0;
    }

    private sealed class Block(DataSpace space)
    {
        private readonly Memory<byte> data = new byte[space.blockSize];

        public long NextBlock
        {
            get => BitConverter.ToInt64(data.Span);
            set => BitConverter.TryWriteBytes(data.Span, value);
        }

        public ushort ContentStop
        {
            get => BitConverter.ToUInt16(data[8..].Span);
            set
            {
                BitConverter.TryWriteBytes(data[8..].Span, value);
                UpdateContent();
            }
        }

        public Memory<byte> Content { get; private set; }

        public async ValueTask Read(long id)
        {
            MoveToBlock(id);
            await space.stream.ReadAsync(data);
            UpdateContent();
        }

        public async ValueTask Write(long index)
        {
            MoveToBlock(index);
            await space.stream.WriteAsync(data);
        }

        private void MoveToBlock(long index)
            => space.stream.Position = space.GetBlockOffset(index);

        private void UpdateContent()
            => Content = data[new Range(new Index(10), ContentStop == 0 ? Index.End : new Index(ContentStop))];
    }

    private sealed class Editor(DataSpace space) : IDataSpaceEditor
    {
        private readonly SequentialExecutor executor = new();

        public async ValueTask<long> Create()
            => await executor.Execute(async () =>
            {
                long id = await Allocate();
                await space.WriteStopCount(id, 0);
                return id;
            });

        public async ValueTask Save(long id, ReadOnlyMemory<byte> source)
            => await executor.Execute(async () =>
            {
                await space.block.Read(id);


            });

        public async ValueTask Delete(long id)
            => await executor.Execute(async () => await Free(id));

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
            else
            {
                space.header.LastFreeBlock = index;
            }
        }
    }
}