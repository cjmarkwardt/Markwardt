namespace Markwardt;

[ServiceType<IFolder>]
public class DesktopFolderTag : LocalSpecialFolderTag
{
    public override Environment.SpecialFolder Folder => Environment.SpecialFolder.Desktop;
}