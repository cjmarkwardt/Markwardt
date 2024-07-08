namespace Markwardt;

public class DataBlocks
{
    public bool IsFree { get; set; } = true;
    public int? Previous { get; set; }
    public int? Next { get; set; }

    public Memory<byte> Data { get; }
}