namespace Markwardt;

public interface IEntity
{
    int Id { get; }

    bool Contains(Type type);

    bool Contains<T>()
        => Contains(typeof(T));

    object Add(Type type);

    T Add<T>()
        => (T)Add(typeof(T));

    object Get(Type type);

    T Get<T>()
        => (T)Get(typeof(T));

    void Delete(Type type);

    void Delete<T>()
        => Delete(typeof(T));

    Maybe<object> MaybeGet(Type type)
        => Contains(type) ? Get(type) : new Maybe<object>();

    Maybe<T> MaybeGet<T>()
        => Contains<T>() ? Get<T>() : new Maybe<T>();

    object GetOrAdd(Type type)
        => Contains(type) ? Get(type) : Add(type);

    T GetOrAdd<T>()
        => Contains<T>() ? Get<T>() : Add<T>();
}