namespace Markwardt;

public interface ICompositeDisposable : ISet<object>, IMultiDisposable;

public class CompositeDisposable : HashSet<object>, ICompositeDisposable
{
    public void Dispose()
        => this.TryDisposeAll();

    public async ValueTask DisposeAsync()
        => await this.TryDisposeAllAsync();
}