namespace Markwardt;

[ServiceType<IFolder>]
public class RootFolderTag : SourceTag<SystemFolderTag, IFolder>
{
    protected override ValueTask<object> Create(IFolder source)
        => ValueTask.FromResult<object>(source.AscendRoot());
}