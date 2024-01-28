namespace Markwardt;

[ServiceType(typeof(IFolder))]
public class CurrentFolderTag : LocalFileNodeTag
{
    public override string Path => Environment.CurrentDirectory;
}