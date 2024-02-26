namespace Markwardt;

public class JsonDataTransformerTag : ImplementationTag<JsonDataTransformer>;

public class JsonDataTransformer : ITransformer<DataObject>
{
    public ValueTask<Failable<DataObject>> Deserialize(Stream input, CancellationToken cancellation = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask<Failable> Serialize(DataObject data, Stream output, CancellationToken cancellation = default)
    {
        throw new NotImplementedException();
    }
}