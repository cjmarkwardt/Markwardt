namespace Markwardt;

public class DataHeaderWriter(IDataSpaceWriter writer)
{
    public static int Size => sizeof(int) * 3;

    private readonly DataBlockWriter blockHeader = new(writer, true);
    private readonly Memory<byte> data = new byte[Size];

    private bool isRead;
    private bool isModified;
    private int newBlock;
    private int firstFreeBlock;
    private int lastFreeBlock;

    public async ValueTask Initialize()
    {
        isRead = true;
        isModified = true;
        newBlock = 1;
        firstFreeBlock = -1;
        lastFreeBlock = -1;
        await Write();

        blockHeader.SetFinal(0);
        await blockHeader.Write(0);
    }

    public async ValueTask WriteNextBlock(int index, int nextBlock)
    {
        blockHeader.SetContinuation(nextBlock);
        await blockHeader.Write(index);
    }

    public async ValueTask<int> ReadNextBlock(int index)
    {
        await blockHeader.Read(index);
        return blockHeader.NextBlock;
    }

    public async ValueTask<int> Allocate()
    {
        await Read();

        int index;
        if (firstFreeBlock != -1)
        {
            index = firstFreeBlock;

            if (lastFreeBlock == index)
            {
                firstFreeBlock = -1;
                lastFreeBlock = -1;
            }
            else
            {
                firstFreeBlock = await ReadNextBlock(index);
            }
        }
        else
        {
            index = newBlock;
            newBlock++;

            await WriteNextBlock(index, -1);
        }

        isModified = true;

        return index;
    }

    public async ValueTask Free(int index)
    {
        await Read();
        
        if (lastFreeBlock == -1)
        {
            firstFreeBlock = index;
            lastFreeBlock = index;
            isModified = true;
        }
        else
        {
            await WriteNextBlock(lastFreeBlock, index);
        }
    }

    public async ValueTask Write()
    {
        if (isRead && isModified)
        {
            isModified = false;

            BitConverter.TryWriteBytes(data.Span, newBlock);
            BitConverter.TryWriteBytes(data.Span[4..], firstFreeBlock);
            BitConverter.TryWriteBytes(data.Span[8..], lastFreeBlock);

            await writer.WriteHeader(data);
        }
    }

    private async ValueTask Read()
    {
        if (!isRead)
        {
            isRead = true;

            await writer.ReadHeader(data);

            newBlock = BitConverter.ToInt32(data.Span);
            firstFreeBlock = BitConverter.ToInt32(data.Span[4..]);
            lastFreeBlock = BitConverter.ToInt32(data.Span[8..]);
        }
    }
}