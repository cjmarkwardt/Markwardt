namespace Markwardt;

[ServiceType(typeof(IFolder))]
public class ApplicationDataFolderTag : LocalFileNodeTag
{
    public override string Path => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
}