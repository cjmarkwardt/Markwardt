namespace Markwardt;

public class DataSegment(IDataSegmentTyper segmentTyper, IDataHandler handler, DataDictionary data, Type type) : DynamicObject
{
    private readonly Dictionary<string, object?> wrappers = [];

    public static T Adapt<T>(IDataSegmentTyper segmentTyper, IDataHandler handler, DataDictionary data)
        where T : class
        => Impromptu.ActLike<T>(new DataSegment(segmentTyper, handler, data, typeof(T)));

    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        string name = binder.Name;

        if (wrappers.TryGetValue(name, out result))
        {
            return true;
        }

        ITypeDescription? property = GetPropertyDescription(name);
        if (property is not null)
        {
            result = CreateWrapper(property, name);
            if (result is not null)
            {
                wrappers.Add(name, result);
                return true;
            }

            result = handler.Deserialize(property.Type, data.Get(name)?.AsValue()?.Content);
            return true;
        }

        result = null;
        return false;
    }

    public override bool TrySetMember(SetMemberBinder binder, object? value)
    {
        string name = binder.Name;
        PropertyInfo? property = GetPropertyInfo(name);
        if (property is not null)
        {
            string? valueData = handler.Serialize(property.PropertyType, value);
            data.Set(name, valueData is null ? null : new DataValue(valueData));
            return true;
        }

        return false;
    }

    private PropertyInfo? GetPropertyInfo(string name)
        => type.GetInterfaces().Prepend(type).SelectMany(x => x.GetProperties()).FirstOrDefault(x => x.Name == name);

    private ITypeDescription? GetPropertyDescription(string name)
    {
        PropertyInfo? property = GetPropertyInfo(name);
        return property is null ? null : new TypeTarget(property).Description;
    }

    private DataList GetList(string name)
    {
        if (data.TryGetValue(name, out IDataNode? node) && node is DataList list)
        {
            return list;
        }
        else
        {
            list = [];
            data[name] = list;
            return list;
        }
    }

    private DataDictionary GetDictionary(string name)
    {
        if (data.TryGetValue(name, out IDataNode? node) && node is DataDictionary dictionary)
        {
            return dictionary;
        }
        else
        {
            dictionary = [];
            data[name] = dictionary;
            return dictionary;
        }
    }

    private object? CreateWrapper(ITypeDescription property, string name)
    {
        if (property.Type.HasGenericTypeDefinition(typeof(IList<>)))
        {
            ITypeDescription item = property.Parameters.First();
            if (!item.IsNullable)
            {
                return SegmentValueList.Create(item.Type, handler, GetList(name));
            }
        }
        else if (property.Type.HasGenericTypeDefinition(typeof(IDictionary<,>)))
        {
            ITypeDescription key = property.Parameters.ElementAt(0);
            ITypeDescription item = property.Parameters.ElementAt(1);
            if (!key.IsNullable && !item.IsNullable)
            {
                return SegmentValueDictionary.Create(key.Type, item.Type, handler, GetDictionary(name));
            }
        }
        else if (property.Type.HasGenericTypeDefinition(typeof(ISegmentSlot<>)))
        {
            ITypeDescription item = property.Parameters.First();
            if (!item.IsNullable)
            {
                return SegmentSlot.Create(item.Type, segmentTyper, handler, data, name);
            }
        }
        else if (property.Type.HasGenericTypeDefinition(typeof(ISegmentList<>)))
        {
            ITypeDescription item = property.Parameters.First();
            if (!item.IsNullable)
            {
                return SegmentList.Create(item.Type, segmentTyper, handler, GetList(name));
            }
        }
        else if (property.Type.HasGenericTypeDefinition(typeof(ISegmentDictionary<,>)))
        {
            ITypeDescription key = property.Parameters.ElementAt(0);
            ITypeDescription item = property.Parameters.ElementAt(1);
            if (!key.IsNullable && !item.IsNullable)
            {
                return SegmentDictionary.Create(key.Type, item.Type, segmentTyper, handler, GetDictionary(name));
            }
        }

        return null;
    }
}