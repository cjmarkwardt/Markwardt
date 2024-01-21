namespace Markwardt;

public interface IExportPlan
{
    ValueTask<IExportNode> Export(IServiceResolver services, object? instance);
    ValueTask<object?> Import(IServiceResolver services, IExportNode export);
}