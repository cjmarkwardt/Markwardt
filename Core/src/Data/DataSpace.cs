namespace Markwardt;

public interface IDataSpace
{
    ValueTask Load(int id, IDynamicBuffer destination);
    ValueTask Edit(Func<IDataSpaceEditor, ValueTask> edit);
}

public interface IDataSpaceEditor
{
    ValueTask<int> Create(ReadOnlyMemory<byte>? source = null);
    ValueTask Save(int id, ReadOnlyMemory<byte> source);
    ValueTask Delete(int id);
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
    private static ReadOnlyMemory<byte> CreateEmptyBlock()
    {
        Memory<byte> block = new byte[6];
        BitConverter.TryWriteBytes(block.Span, -1);
        BitConverter.TryWriteBytes(block[4..].Span, (ushort)0);
        return block;
    }

    public DataSpace(Stream stream, int blockLength)
    {
        this.stream = stream;
        this.blockLength = blockLength;

        header = new(this);
        blockHeader = new(this);
        block = new(this);
        editor = new(this);
    }

    private readonly Stream stream;
    private readonly int blockLength;
    private readonly Memory<byte> blockB;
    private readonly Header header;
    private readonly BlockHeader blockHeader;
    private readonly Block block;
    private readonly Editor editor;
    private readonly ReadOnlyMemory<byte> emptyBlock = CreateEmptyBlock();
    private readonly SequentialExecutor executor = new();

    private bool isHeaderRead;

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
    
    private long GetBlockOffset(int index)
        => Header.Size + ((long)index * blockLength);

    private void MoveToBlock(int index)
        => stream.Position = GetBlockOffset(index);

    private readonly Memory<byte> blockBuffer = new byte[100];

    private async ValueTask WriteBlock(int index, ReadOnlyMemory<byte> source)
    {
        bool isFinal = source.Length <= blockLength - 5;

        int parameter = isFinal ? source.Length : 

        blockBuffer.Span.WriteBool(isFinal, out _);

    }

    private async ValueTask Read(long position, Memory<byte> destination)
    {
        long originalPosition = stream.Position;
        stream.Position = position;
        await stream.ReadAsync(destination);
        stream.Position = originalPosition;
    }

    private async ValueTask Write(long position, ReadOnlyMemory<byte> source)
    {
        long originalPosition = stream.Position;
        stream.Position = position;
        await stream.WriteAsync(source);
        stream.Position = originalPosition;
    }

    private sealed class Header(DataSpace space)
    {
        public static int Size => 4 + 4 + 4;

        private readonly Memory<byte> data = new byte[Size];

        public int NewBlock { get; set; }
        public int FirstFreeBlock { get; set; }
        public int LastFreeBlock { get; set; }

        public async ValueTask Read()
        {
            await space.Read(0, data);
            NewBlock = BitConverter.ToInt32(data.Span);
            FirstFreeBlock = BitConverter.ToInt32(data[4..].Span);
            LastFreeBlock = BitConverter.ToInt32(data[8..].Span);
        }

        public async ValueTask Write()
        {
            BitConverter.TryWriteBytes(data.Span, NewBlock);
            BitConverter.TryWriteBytes(data[4..].Span, FirstFreeBlock);
            BitConverter.TryWriteBytes(data[8..].Span, LastFreeBlock);
            await space.Write(0, data);
        }

        public async ValueTask<int> Allocate()
        {
            int index;
            if (FirstFreeBlock != -1)
            {
                index = FirstFreeBlock;

                if (LastFreeBlock == index)
                {
                    FirstFreeBlock = -1;
                    LastFreeBlock = -1;
                }
                else
                {
                    await space.blockHeader.Read(index);
                    FirstFreeBlock = space.block.NextBlock;
                }
            }
            else
            {
                index = NewBlock;
                NewBlock++;
            }

            return index;
        }

        public async ValueTask Free(int index)
        {
            if (LastFreeBlock != -1)
            {
                await space.blockHeader.Read(LastFreeBlock);
                space.block.NextBlock = index;
                await space.blockHeader.Write(LastFreeBlock);
            }
            
            LastFreeBlock = index;
        }
    }

    private abstract class BaseBlock(DataSpace space, int length)
    {
        public static int HeaderLength => 4 + 2;

        protected DataSpace Space { get; } = space;
        protected Memory<byte> Data { get; } = new byte[length];

        public bool IsFinal { get; set; }
        public int Parameter { get; set; }

        public Memory<byte> Content => Data[HeaderLength..];

        protected async ValueTask Read(int index, bool includeContent)
        {
            await Space.Read(Space.GetBlockOffset(index), GetTargetData(includeContent));
            IsFinal = Data.Span.AsReadOnly().ReadBool(out _);
            Parameter = Data.Span
            NextBlock = BitConverter.ToInt32(Data.Span); 
            ContentLength = BitConverter.ToUInt16(Data[4..].Span);
        }

        protected async ValueTask Write(int index, bool includeContent)
        {
            BitConverter.TryWriteBytes(Data.Span, NextBlock);
            BitConverter.TryWriteBytes(Data[4..].Span, ContentLength);
            await Space.Write(Space.GetBlockOffset(index), GetTargetData(includeContent));
        }

        private Memory<byte> GetTargetData(bool includeContent)
            => includeContent ? Data[..(HeaderLength + ContentLength)] : Data[..HeaderLength];
    }

    private sealed class BlockHeader(DataSpace space) : BaseBlock(space, HeaderLength)
    {
        public async ValueTask Read(int index)
            => await Read(index, false);

        public async ValueTask Write(int index)
            => await Write(index, false);
    }

    private sealed class Block(DataSpace space) : BaseBlock(space, space.blockLength)
    {
        public int MaxContentLength => Space.blockLength - HeaderLength;

        public new async ValueTask Read(int index, bool readContent)
            => await base.Read(index, readContent);

        public new async ValueTask Write(int index, bool writeContent)
            => await base.Write(index, writeContent);
    }

    private sealed class Editor(DataSpace space) : IDataSpaceEditor
    {
        private readonly SequentialExecutor executor = new();

        public async ValueTask<int> Create(ReadOnlyMemory<byte>? source = null)
            => await executor.Execute(async () =>
            {
                int id = await space.header.Allocate();
                await WriteBlocks(id, source);
                return id;
            });

        public async ValueTask Save(int id, ReadOnlyMemory<byte> source)
            => await executor.Execute(async () => await WriteBlocks(id, source));

        public async ValueTask Delete(int id)
            => await executor.Execute(async () => await space.header.Free(id));

        private async ValueTask WriteBlocks(int id, ReadOnlyMemory<byte>? source)
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
    }
}