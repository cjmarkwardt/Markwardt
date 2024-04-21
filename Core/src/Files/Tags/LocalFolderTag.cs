namespace Markwardt;

public abstract class LocalFolderTag : LocalFileNodeTag
{
    protected override async ValueTask<object> Create(IFileTree source)
        => ((IFileNode)await base.Create(source)).AsFolder();
}