namespace Markwardt;

[ServiceType(typeof(IFolder))]
public class DocumentsFolderTag : LocalFileNodeTag
{
    public override string Path => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
}