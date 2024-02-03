namespace Markwardt;

public enum FileOpenMode
{
    /// <summary> Opens the file at the beginning, or fails if the file doesn't exist. </summary>
    Open,

    /// <summary> Creates a new file, or fails if the file already exists. </summary>
    Create,

    /// <summary> Opens the file at the beginning, or creates a new file if it doesn't exist. </summary>
    OpenOrCreate,

    /// <summary> Opens the file and truncates it, or creates a new file if it doesn't exist. </summary>
    Overwrite,

    /// <summary> Opens the file at the end, or creates a new file if it doesn't exist. </summary>
    Append
}