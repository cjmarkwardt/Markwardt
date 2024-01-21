namespace Markwardt;

public interface IConfigurableExportPlan : IExportPlan
{
    void ConfigureTypeName(string typeName);
    void ConfigureValues(AsyncFunc<IServiceResolver, object?, IEnumerable<IExportValue?>> export, AsyncAction<IServiceResolver, object?, IEnumerable<IExportValue?>> import);
    void ConfigureProperty(string property, AsyncFunc<IServiceResolver, object?, IExportNode?> export, AsyncAction<IServiceResolver, object?, IExportNode?> import);
}

public class ReflectionExportPlan : IConfigurableExportPlan
{
    private readonly Dictionary<string, PropertyConfiguration> properties = [];

    private string? typeName;
    private ValuesConfiguration? values;

    public async ValueTask<IExportNode> Export(IServiceResolver services, object? instance)
    {
        
    }

    public async ValueTask<object?> Import(IServiceResolver services, IExportNode export)
    {
        throw new NotImplementedException();
    }

    public void ConfigureTypeName(string typeName)
        => this.typeName = typeName;

    public void ConfigureValues(AsyncFunc<IServiceResolver, object?, IEnumerable<IExportValue?>> export, AsyncAction<IServiceResolver, object?, IEnumerable<IExportValue?>> import)
        => values = new(export, import);

    public void ConfigureProperty(string property, AsyncFunc<IServiceResolver, object?, IExportNode?> export, AsyncAction<IServiceResolver, object?, IExportNode?> import)
        => properties[property] = new(export, import);

    private record ValuesConfiguration(AsyncFunc<IServiceResolver, object?, IEnumerable<IExportValue?>> Export, AsyncAction<IServiceResolver, object?, IEnumerable<IExportValue?>> Import);
    private record PropertyConfiguration(AsyncFunc<IServiceResolver, object?, IExportNode?> Export, AsyncAction<IServiceResolver, object?, IExportNode?> Import);
}
