namespace Markwardt;

public class CompositeModuleRepository(IEnumerable<IModuleRepository> repositories) : Component, IModuleRepository
{
    private readonly List<IModuleRepository> repositories = repositories.ToList();


    public IReadOnlyDictionary<string, IModule> Modules => throw new NotImplementedException();

    public async ValueTask Refresh(bool purge = true)
        => await Task.WhenAll(repositories.Select(x => x.Refresh(purge).AsTask()));
}
