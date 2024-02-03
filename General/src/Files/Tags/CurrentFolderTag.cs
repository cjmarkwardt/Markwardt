namespace Markwardt;

[ServiceType(typeof(IFolder))]
public class CurrentFolderTag : LocalFolderTag
{
    public override string Path => Environment.CurrentDirectory;
}