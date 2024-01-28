namespace Markwardt;

[ServiceType(typeof(IFolder))]
public class DesktopFolderTag : LocalFileNodeTag
{
    public override string Path => Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
}