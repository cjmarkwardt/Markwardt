namespace Markwardt;

public interface IMaybeEnumerator<T>
{
    Maybe<T> Current { get; }
    
    Maybe<T> Next();
}

public static class MaybeEnumerableExtensions
{
    public static IMaybeEnumerator<T> GetMaybeEnumerator<T>(this IEnumerable<T> enumerable)
        => new MaybeEnumerator<T>(enumerable.GetEnumerator());
}

public class MaybeEnumerator<T> : IMaybeEnumerator<T>
{
    public MaybeEnumerator(IEnumerator<T> enumerator)
    {
        this.enumerator = enumerator;

        Next();
    }

    private readonly IEnumerator<T> enumerator;
    
    public Maybe<T> Current { get; private set; }

    public Maybe<T> Next()
        => Current = enumerator.MoveNext() ? enumerator.Current : new Maybe<T>();
}