namespace Markwardt;

public class DataSegmentReader<T>(IDataSegmentTyper segmentTyper, IDataHandler handler)
{
    public TDerived Read<TDerived>(IDataNode node)
        where TDerived : class, T
        => segmentTyper.Is(node, typeof(TDerived)) ? DataSegment.Adapt<TDerived>(segmentTyper, handler, node.AsDictionary().NotNull()) : throw new InvalidOperationException($"Data segment is not of type {typeof(TDerived)}");
}