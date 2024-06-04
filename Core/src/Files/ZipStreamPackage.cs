using System.IO.Compression;

namespace Markwardt;

[Factory<ZipStreamPackage>]
public delegate ValueTask<IStreamPackage> ZipStreamPackageFactory(Func<ZipArchive> open);

public class ZipStreamPackage(Func<ZipArchive> open) : IStreamPackage
{
    public IDisposable<Stream>? Open(string name)
    {
        ZipArchive archive = open();
        ZipArchiveEntry? entry = archive.GetEntry(name);
        if (entry is null)
        {
            return null;
        }

        return new Disposable<Stream>(entry.Open(), [archive]);
    }
}