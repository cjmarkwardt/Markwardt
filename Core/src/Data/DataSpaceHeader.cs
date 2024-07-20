namespace Markwardt;

public class DataSpaceHeader(IDataSpaceWriter writer)
{
    public static async ValueTask<DataSpaceHeader> Create(IDataSpaceWriter writer)
    {
        DataSpaceHeader header = new(writer);
        await header.Read();
        return header;
    }

    private readonly DataBlock blockHeader = new(writer, true);
    private readonly Memory<byte> data = new byte[writer.HeaderSize];

    public int NewBlock { get; set; }
    public int FirstFreeBlock { get; set; }
    public int LastFreeBlock { get; set; }

    public async ValueTask Read()
    {
        await writer.ReadHeader(data);

        NewBlock = data.Span.AsReadOnly().ReadInt(out _);
        FirstFreeBlock = data[4..].Span.AsReadOnly().ReadInt(out _);
        LastFreeBlock = data[8..].Span.AsReadOnly().ReadInt(out _);
    }

    public async ValueTask Write()
    {
        data.Span.WriteInt(NewBlock, out _);
        data[4..].Span.WriteInt(FirstFreeBlock, out _);
        data[8..].Span.WriteInt(LastFreeBlock, out _);

        await writer.WriteHeader(data);
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
                await blockHeader.Read(index);
                FirstFreeBlock = blockHeader.Parameter;
            }
        }
        else
        {
            index = NewBlock;
            NewBlock++;

            blockHeader.Parameter = -1;
            await blockHeader.Write(index);
        }

        return index;
    }

    public async ValueTask Free(int index)
    {
        if (LastFreeBlock == -1)
        {
            FirstFreeBlock = index;
            LastFreeBlock = index;
        }
        else
        {
            await blockHeader.Read(LastFreeBlock);
            blockHeader.Parameter = index;
            await blockHeader.Write(LastFreeBlock);
        }
    }
}