using System.IO.Compression;

namespace Markwardt;

[Factory<ZipAccessor>]
public delegate ValueTask<IDataAccessor<string, Stream?>> ZipAccessorFactory(Func<ZipArchive> open);

public class ZipAccessor(Func<ZipArchive> open) : IDataAccessor<string, Stream?>
{
    public async ValueTask Access(string key, Func<Stream?, ValueTask> read)
    {
        ZipArchive archive = open();
        ZipArchiveEntry? entry = archive.GetEntry(key);
        await using Stream? data = entry?.Open();
        await read(data);
    }
}