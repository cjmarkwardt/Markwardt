namespace Markwardt;

public interface IStreamTransformer<T> : IStreamSerializer<T>, IStreamDeserializer<T>;