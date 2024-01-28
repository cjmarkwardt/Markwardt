namespace Markwardt;

public static class StringExtensions
{
    private static readonly char[] separators = ['/', '\\'];

    public static IList<string> SplitPath(this string path)
        => path.Split(separators);
}