namespace Markwardt;

[ServiceType<IFolder>]
public class CurrentFolderTag : LocalFolderTag
{
    public override string Path => Environment.CurrentDirectory;
}