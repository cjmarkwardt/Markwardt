namespace Markwardt;

[ServiceType(typeof(IFolder))]
public class ApplicationDataFolderTag : LocalSpecialFolderTag
{
    public override Environment.SpecialFolder Folder => Environment.SpecialFolder.ApplicationData;
}