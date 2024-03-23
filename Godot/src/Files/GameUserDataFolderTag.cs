namespace Markwardt.Godot;

public class GameUserDataFolderTag : LocalFolderTag
{
    public override string Path => OS.GetUserDataDir();
}