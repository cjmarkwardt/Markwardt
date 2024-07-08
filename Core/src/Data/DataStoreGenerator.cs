namespace Markwardt;

public interface IDataStoreGenerator
{
    ValueTask<IDataStore> Create(Stream stream);
    ValueTask<IDataStore> Load(Stream stream);
}

public class DataStoreGenerator : IDataStoreGenerator
{
    public async ValueTask<IDataStore> Create(Stream stream)
    {
        
    }

    public ValueTask<IDataStore> Load(Stream stream)
    {
        throw new NotImplementedException();
    }
}