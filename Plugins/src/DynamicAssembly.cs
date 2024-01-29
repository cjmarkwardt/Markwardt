namespace Markwardt;

public interface IDynamicAssembly : IComponent
{
    [Factory<DynamicAssembly>]
    delegate ValueTask<IDynamicAssembly> Factory(IFile file, IEnumerable<Type> sharedTypes);

    Assembly Value { get; }
    bool IsLoaded { get; }

    bool Load();
    void Reload();
    bool Unload();
}

public class DynamicAssembly(IFile file, IEnumerable<Type> sharedTypes) : Component, IDynamicAssembly
{
    private readonly Type[] sharedTypes = sharedTypes.ToArray();

    private PluginLoader? loader;

    private Assembly? value;
    public Assembly Value
    {
        get
        {
            Load();
            return value;
        }
    }

    public bool IsLoaded => value != null;

    [MemberNotNull(nameof(value))]
    public bool Load()
    {
        Verify();

        if (value == null)
        {
            Reload();
            return true;
        }

        return false;
    }

    [MemberNotNull(nameof(value))]
    public void Reload()
    {
        Verify();

        value = null;

        if (loader == null)
        {
            loader = PluginLoader.CreateFromAssemblyFile(file.GetLocalPath().NotNull($"Assembly file must be local at {file}"), true, sharedTypes);
        }
        else
        {
            loader.Reload();
        }
        
        value = loader.LoadDefaultAssembly();
    }

    public bool Unload()
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

    protected override void OnSharedDisposal()
    {
        base.OnSharedDisposal();

        Unload();
    }
}