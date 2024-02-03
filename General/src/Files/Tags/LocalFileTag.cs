namespace Markwardt;

public abstract class LocalFileTag : LocalFileNodeTag
{
    protected override async ValueTask<object> Create(IFileTree source)
        => ((IFileNode)await base.Create(source)).AsFile();
}