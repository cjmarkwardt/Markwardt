namespace Markwardt;

public static class StringExtensions
{
    private static readonly char[] pathSeparators = ['/', '\\'];

    public static IList<string> SplitPath(this string path)
        => path.Split(pathSeparators);

    public static string AsLocalPath(this string path)
        => path.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);

    public static string Transform(this string? text, Func<string, string> transform)
        => text == null ? string.Empty : transform(text);
}