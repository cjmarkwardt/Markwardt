namespace Markwardt;

public interface IBlockSource
{
    IDataPointer GetBlock(int index);

}

public class BlockPointer(IDataPointer data, int blockSize, int baseBlock, int block, int position, Func<ValueTask<int>> createBlock) : IDataPointer
{
    public int Position => position;

    public IDataPointer Move(int offset)
        => this.LazyMove(offset, async () => new BlockPointer(data, blockSize, baseBlock, await FindBlock(position + offset), position + offset, createBlock));

    public ValueTask<IDataPointer> Read(Memory<byte> destination, int length, int offset = 0, bool move = false)
    {
        throw new NotImplementedException();
    }

    public ValueTask<IDataPointer> Write(ReadOnlyMemory<byte> source, int offset = 0, bool move = false)
    {
        throw new NotImplementedException();
    }

    private int GetSequence(int position)
        => position / blockSize;

    private async ValueTask<int> FindBlock(int position)
    {
        int currentBlock = block;
        int currentSequence = GetSequence(Position);
        int targetSequence = GetSequence(position);

        if (targetSequence == currentSequence)
        {
            return currentBlock;
        }
        else if (targetSequence == 0)
        {
            return baseBlock;
        }
        else if (targetSequence < currentSequence)
        {
            currentBlock = baseBlock;
            currentSequence = 0;
        }

        while (currentSequence < targetSequence)
        {
            currentBlock = await ReadNextBlock(currentBlock);
            if (currentBlock == -1)
            {
                currentBlock = await createBlock();
            }

            currentSequence++;
        }
    }

    private async ValueTask<int> ReadNextBlock(int block)
        => (await data.ReadInteger(block * blockSize)).Value;

    /*private readonly DirectBlockPointer data = new();

    public int Position { get; set; }

    public IDataPointer Copy()
    {
        throw new NotImplementedException();
    }

    public ValueTask Read(int length, Memory<byte> destination, bool move = false)
    {
        int read = 0;
        int readingBlock = currentBlock;
        while (length > 0)
        {
            int nextRead = Math.Min(contentSize, length);
            int blockRemainder = Position % contentSize;
        }
    }

    public ValueTask Write(ReadOnlyMemory<byte> source, bool move = false)
    {
        throw new NotImplementedException();
    }*/
}

public interface IIdSpace
{
    ValueTask<IList<int>> Allocate(int count);
    ValueTask Free(IEnumerable<int> ids);
}

public class IdSpace(IDataPointer data) : IIdSpace
{
    private static int NextIdOffset => 0;
    private static int AvailableIdCountOffset => 4;

    private static int GetAvailableIdOffset(int index)
        => 8 + (4 * index);

    public async ValueTask<IList<int>> Allocate(int count)
    {
        IList<int> ids = await PopAvailableIds(count);
        if (ids.Count < count)
        {
            ids.Add(await GenerateIds(count - ids.Count));
        }

        return ids;
    }

    public async ValueTask Free(IEnumerable<int> ids)
        => await PushAvailableIds(ids);

    private async ValueTask<int> ReadNextId()
        => await data.ReadInteger(NextIdOffset);

    private async ValueTask WriteNextId(int value)
        => await data.WriteInteger(value, NextIdOffset);

    private async ValueTask<int> ReadAvailableIdCount()
        => await data.ReadInteger(AvailableIdCountOffset);

    private async ValueTask WriteAvailableIdCount(int value)
        => await data.WriteInteger(value, AvailableIdCountOffset);

    private async ValueTask<IList<int>> ReadAvailableIds(int index, int count)
        => await data.ReadIntegers(count, GetAvailableIdOffset(index));

    private async ValueTask WriteAvailableIds(int index, IEnumerable<int> values)
        => await data.WriteIntegers(values, GetAvailableIdOffset(index));

    private async ValueTask<IList<int>> PopAvailableIds(int count)
    {
        int availableIdCount = await ReadAvailableIdCount();
        if (count > availableIdCount)
        {
            count = availableIdCount;
        }

        availableIdCount -= count;

        IList<int> ids = await ReadAvailableIds(availableIdCount, count);
        await WriteAvailableIdCount(availableIdCount);
        return ids;
    }

    private async ValueTask PushAvailableIds(IEnumerable<int> values)
    {
        int availableIdCount = await ReadAvailableIdCount();
        await WriteAvailableIds(availableIdCount, values);
        await WriteAvailableIdCount(availableIdCount + values.Count());
    }

    private async ValueTask<IList<int>> GenerateIds(int count)
    {
        List<int> ids = new(count);
        int nextId = await ReadNextId();
        while (ids.Count < count)
        {
            ids.Add(nextId);
            nextId++;
        }

        await WriteNextId(nextId);
        return ids;
    }
}

public interface IDataSpace
{
    ValueTask<IDataFilePointer> Create(int size = 0);
    IDataFilePointer Open(int id);
    ValueTask Resize(int id, int size);
    ValueTask Delete(int id);
}

public interface IDataSpaceGenerator
{
    ValueTask<IDataSpace> Create(IDataPointer data, int blockDataSize, bool isFixedSize);
}

public class DataSpace(IDataPointer data, int blockDataSize, bool isFixedSize) : IDataSpace
{
    private readonly Lazy<byte[]> empty = new(() => new byte[blockDataSize]);

    private bool isInitialized;

    private IDataPointer Data { get; } = data;
    private int BlockSize { get; } = blockDataSize;
    private bool IsFixedSize { get; } = isFixedSize;
    
    public async ValueTask<IDataFilePointer> Create(int size = 0)
    {
        int block = await AllocateBlock();

    }

    public IDataFilePointer Open(int id)
    {
        throw new NotImplementedException();
    }

    public ValueTask Resize(int id, int size)
    {
        throw new NotImplementedException();
    }

    public ValueTask Delete(int id)
    {
        throw new NotImplementedException();
    }

    private async ValueTask<Header> ReadHeader()
    {
        if (!isInitialized)
        {
            isInitialized = true;

            if (! await Data.Test(12))
            {
                Header header = new();
                await WriteHeader(header);
                return header;
            }
        }

        IList<int> values = await Data.ReadIntegers(3);
        return new Header { NewBlock = values[0], FirstFreeBlock = values[1], LastFreeBlock = values[2] };
    }

    private async ValueTask WriteHeader(Header header)
        => await Data.WriteIntegers([header.NewBlock, header.FirstFreeBlock, header.LastFreeBlock]);

    private long GetBlockOffset(int block)
        => 8 + (block * BlockSize);

    private async ValueTask<int> ReadBlockNext(int block)
        => await Data.ReadInteger(GetBlockOffset(block));

    private async ValueTask WriteBlockNext(int block, int nextBlock)
        => await Data.WriteInteger(nextBlock, GetBlockOffset(block));

    private async ValueTask<int> ReadFileLength(int fileId)
        => await Data.ReadInteger(GetBlockOffset(fileId));

    private async ValueTask WriteFileLength(int fileId, int length)
        => await Data.WriteInteger(length, GetBlockOffset(fileId));

    protected IDataPointer CreateBlockPointer(int block)
        => Data.Copy().Move(8 + (block * BlockSize));

    private async ValueTask<int> AllocateBlock()
    {
        int block;
        Header header = await ReadHeader();
        if (header.FirstFreeBlock != -1)
        {
            block = header.FirstFreeBlock;
            Data.write

            if (header.LastFreeBlock == block)
            {
                header.FirstFreeBlock = -1;
                header.LastFreeBlock = -1;
            }
            else
            {
                header.FirstFreeBlock = await ReadBlockNext(block);
            }
        }
        else
        {
            block = header.NewBlock;
            header.NewBlock++;
        }

        await WriteHeader(header);
        return block;
    }

    private async ValueTask FreeBlock(int block)
    {
        Header header = await ReadHeader();
        if (header.LastFreeBlock != -1)
        {
            await WriteBlockNext(header.LastFreeBlock, block);
        }
        else
        {
            header.LastFreeBlock = block;
        }

        await WriteHeader(header);
    }

    private class Header
    {
        public int NewBlock { get; set; } = 0;
        public int FirstFreeBlock { get; set; } = -1;
        public int LastFreeBlock { get; set; } = -1;
    }

    private sealed class FilePointer(DataSpace space, IDataPointer data, int block) : IDataFilePointer
    {
        private DataSpace Space { get; } = space;
        private IDataPointer Data { get; } = data;

        public int Id => block;

        public int Position { get; set; }

        public IDataFilePointer Copy()
            => new FilePointer(Space, Data.Copy(), block);

        public async ValueTask Delete()
            => await Space.Delete(Id);

        public ValueTask Read(Memory<byte> destination, int length, int offset = 0, bool move = false)
        {
            throw new NotImplementedException();
        }

        public ValueTask Resize(int size)
        {
            throw new NotImplementedException();
        }

        public ValueTask Write(ReadOnlyMemory<byte> source, int offset = 0, bool move = false)
        {
            throw new NotImplementedException();
        }

        IDataPointer IDataPointer.Copy()
            => Copy();
    }
}

public abstract class DataSpacel(int blockSize, IDataPointer data) : IDataSpace
{
    private int allocationLength = 
    private int allocationChunkSize = 

    public async ValueTask<(int Id, IDataPointer Data)> Create()
    {
        int id;


        return (id, Open(id));
    }

    public ValueTask Delete(int id)
    {

    }

    public abstract IDataPointer Open(int id);

    protected IDataPointer CreateBlockPointer(int block)
        => data.Copy(blockSize * block);
}

public class FixedDataSpace(int blockSize, IDataPointer data) : DataSpace(blockSize, data)
{
    public override ValueTask<(int Id, IDataPointer Data)> Create()
    {
        throw new NotImplementedException();
    }

    public override IDataPointer Open(int id)
        => CreateBlockPointer(id);

    public override ValueTask Delete(int id)
    {
        throw new NotImplementedException();
    }
}

public class DynamicDataSpace(int blockSize, IDataPointer data) : IDataSpace
{
    public async ValueTask<(int Id, IDataPointer Data)> Create()
    {
        int finalBlock = await data.ReadInteger(false);

    }

    public IDataPointer Open(int block)
    {
        throw new NotImplementedException();
    }

    public ValueTask Delete(int block)
    {
        throw new NotImplementedException();
    }

    private sealed class Pointer(DynamicDataSpace data, int baseBlock) : IDataPointer
    {
        private readonly int baseBlock = baseBlock;

        private int currentBlock = baseBlock;

        public int Position { get; set; }

        public IDataPointer Copy()
            => new Pointer(data, baseBlock) { Position = Position, currentBlock = currentBlock };

        public ValueTask Read(int length, Memory<byte> destination, bool move = false)
        {
            throw new NotImplementedException();
        }

        public ValueTask Write(ReadOnlyMemory<byte> source, bool move = false)
        {
            throw new NotImplementedException();
        }
    }
}