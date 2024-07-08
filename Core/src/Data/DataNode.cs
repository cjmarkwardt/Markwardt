namespace Markwardt;

public interface IDataNode
{
    bool PopChanges();
}

public static class DataNodeExtensions
{
    public static DataValue? AsValue(this IDataNode node)
        => node is DataValue value ? (DataValue?)value : null;

    public static DataList? AsList(this IDataNode node)
        => node as DataList;
    
    public static DataDictionary? AsDictionary(this IDataNode node)
        => node as DataDictionary;
}