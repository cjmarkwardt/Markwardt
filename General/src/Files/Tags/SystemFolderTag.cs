namespace Markwardt;

[ServiceType(typeof(IFolder))]
public class SystemFolderTag : LocalFileNodeTag
{
    public override string Path => Environment.SystemDirectory;
}