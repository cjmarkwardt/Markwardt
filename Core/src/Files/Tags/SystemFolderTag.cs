namespace Markwardt;

[ServiceType<IFolder>]
public class SystemFolderTag : LocalFileNodeTag
{
    public override string Path => Environment.SystemDirectory;
}