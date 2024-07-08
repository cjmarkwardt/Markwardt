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
        List<int> ids = new(count);

        int availableIdCount = await ReadAvailableIdCount();
        while (availableIdCount > 0 && ids.Count < count)
        {
            ids.Add(await ReadAvailableId(availableIdCount - 1));
            availableIdCount--;
        }

        await WriteAvailableIdCount(availableIdCount);

        if (ids.Count < count)
        {
            int nextId = await ReadNextId();
            while (ids.Count < count)
            {
                ids.Add(nextId);
                nextId++;
            }

            await WriteNextId(nextId);
        }

        return ids;
    }

    public async ValueTask Free(IEnumerable<int> ids)
    {
        int availableIdCount = await ReadAvailableIdCount();
        foreach (int id in ids)
        {
            await WriteAvailableId(availableIdCount, id);
            availableIdCount++;
        }

        await WriteAvailableIdCount(availableIdCount);
    }

    private async ValueTask<int> ReadNextId()
        => (await data.ReadInteger(NextIdOffset)).Value;

    private async ValueTask WriteNextId(int value)
        => await data.WriteInteger(value, NextIdOffset);

    private async ValueTask<int> ReadAvailableIdCount()
        => (await data.ReadInteger(AvailableIdCountOffset)).Value;

    private async ValueTask WriteAvailableIdCount(int value)
        => await data.WriteInteger(value, AvailableIdCountOffset);

    private async ValueTask<int> ReadAvailableId(int index)
        => (await data.ReadInteger(GetAvailableIdOffset(index))).Value;

    private async ValueTask WriteAvailableId(int index, int value)
        => await data.WriteInteger(value, GetAvailableIdOffset(index));
}

public interface IDataSpace
{
    ValueTask<(int Id, IDataPointer Data)> Create();
    IDataPointer Open(int id);
    ValueTask Delete(int id);
}

public abstract class DataSpace(int blockSize, IDataPointer data) : IDataSpace
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