namespace Markwardt;

[ServiceType(typeof(IFolder))]
public class DesktopFolderTag : LocalSpecialFolderTag
{
    public override Environment.SpecialFolder Folder => Environment.SpecialFolder.Desktop;
}