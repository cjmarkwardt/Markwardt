namespace Markwardt;

public interface IDynamicAssembly : IComplexDisposable
{
    [Factory<DynamicAssembly>]
    delegate ValueTask<IDynamicAssembly> Factory(IFile file, IEnumerable<Type> sharedTypes);

    Assembly Value { get; }
    bool IsLoaded { get; }

    ValueTask<bool> Load();
    ValueTask Reload();
    ValueTask<bool> Unload();
}

public class DynamicAssembly(IFile file, IEnumerable<Type> sharedTypes) : ComplexDisposable, IDynamicAssembly
{
    private readonly Type[] sharedTypes = sharedTypes.ToArray();
    private readonly SequentialExecutor executor = new();

    private PluginLoader? loader;

    private Assembly? value;
    public Assembly Value => value ?? throw new InvalidOperationException("Not loaded");

    public bool IsLoaded => value != null;

    public async ValueTask<bool> Load()
        => await executor.Execute(() =>
        {
            if (value == null)
            {
                ReloadAssembly();
                return true;
            }

            return false;
        });

    public async ValueTask Reload()
        => await executor.Execute(ReloadAssembly);

    public async ValueTask<bool> Unload()
        => await executor.Execute(UnloadAssembly);

    protected override void PrepareDisposal()
    {
        base.PrepareDisposal();

        executor.DisposeWith(this);
    }

    protected override void OnSharedDisposal()
    {
        base.OnSharedDisposal();

        UnloadAssembly();
    }

    private void ReloadAssembly()
    {
        value = null;

        if (loader == null)
        {
            loader = PluginLoader.CreateFromAssemblyFile(file.GetLocalPath().Result, true, sharedTypes);
        }
        else
        {
            loader.Reload();
        }
        
        value = loader.LoadDefaultAssembly();
    }

    public bool UnloadAssembly()
    {
        if (loader != null)
        {
            value = null;
            loader.Dispose();
            loader = null;
            return true;
        }

        return false;
    }
}