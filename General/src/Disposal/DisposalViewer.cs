namespace Markwardt;

public interface IDisposalViewer : IDisposable
{
    bool IsDisposed { get; }

    void Verify();
}