namespace Markwardt;

public interface IDynamicAssembly : IExtendedDisposable
{
    delegate ValueTask<IDynamicAssembly> Factory(IFile file, IEnumerable<Type> sharedTypes);

    Assembly Value { get; }
    bool IsLoaded { get; }

    ValueTask<bool> Load();
    ValueTask Reload();
    ValueTask<bool> Unload();
}