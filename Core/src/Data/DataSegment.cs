namespace Markwardt;

public class DataSegment(Type type) : DynamicObject
{
    public static 

    private readonly Dictionary<string, object> data = [];

    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        
    }

    public void Serialize(IDataWriter writer)
    {

    }
}