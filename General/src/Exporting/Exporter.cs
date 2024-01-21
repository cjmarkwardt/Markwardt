namespace Markwardt;

public interface IExporter
{
    ValueTask<IExportNode?> Export(object? target, bool includeType = true);
    ValueTask<object?> Import(IExportNode? data, Type type);
}

public class Exporter : IExporter
{
    public async ValueTask<IExportNode?> Export(object? target, bool includeType = true)
    {
        if (target == null)
        {
            return null;
        }

        Type type = target.GetType();

        ExportNode node = new();
    
        if (includeType)
        {
            node.Type = type.GetCustomAttribute<ExportTypeAttribute>()?.Type ?? type.Name;
        }

        foreach (PropertyInfo property in type.GetProperties())
        {
            ExportAttribute? exportAttribute = property.GetCustomAttribute<ExportAttribute>();
            if (exportAttribute != null)
            {
                node.Properties[exportAttribute.Name ?? property.Name] = await Export(property.GetValue(target));
            }
        }

        return node;
    }

    public async ValueTask<object?> Import(IExportNode? data, Type type)
    {
        if (data == null)
        {
            return null;
        }
    }
}