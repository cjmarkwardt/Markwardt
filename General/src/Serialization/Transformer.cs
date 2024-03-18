namespace Markwardt;

public interface ITransformer<T> : ISerializer<T>, IDeserializer<T>;