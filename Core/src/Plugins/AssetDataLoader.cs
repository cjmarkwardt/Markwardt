namespace Markwardt;

public interface IAssetDataLoader : IMultiDisposable
{
    ValueTask<object?> Load(string id, IAssetDataReader? reader = null);
}

public class AssetDataLoader : ExtendedDisposable, IAssetDataLoader
{
    public async ValueTask<object?> Load(string id, IAssetDataReader? reader = null)
    {
        object? asset = await LoadAsset(id);

        if (asset is null && reader is not null)
        {
            using IDisposable<Stream>? stream = await LoadData(id);
            if (stream is not null)
            {
                asset = await reader.Read(id, stream.Value);
            }
        }

        return asset;
    }

    protected virtual ValueTask<object?> LoadAsset(string id) => ValueTask.FromResult<object?>(null);
    protected virtual ValueTask<IDisposable<Stream>?> LoadData(string id) => ValueTask.FromResult<IDisposable<Stream>?>(null);
}