namespace Markwardt;

[ServiceType(typeof(IFolder))]
public class DocumentsFolderTag : LocalSpecialFolderTag
{
    public override Environment.SpecialFolder Folder => Environment.SpecialFolder.MyDocuments;
}