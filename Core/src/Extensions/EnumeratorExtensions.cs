namespace Markwardt;

public static class EnumeratorExtensions
{
    public static Maybe<T> Next<T>(this IEnumerator<T> enumerator)
        => enumerator.MoveNext() ? enumerator.Current.Maybe() : default;
}