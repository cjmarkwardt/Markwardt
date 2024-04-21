namespace Markwardt;

public abstract class LocalSpecialFolderTag : LocalFolderTag
{
    public abstract Environment.SpecialFolder Folder { get; }

    public override sealed string Path => Environment.GetFolderPath(Folder);
}