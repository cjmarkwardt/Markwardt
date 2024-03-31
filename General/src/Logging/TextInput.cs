namespace Markwardt;

public interface ITextInput
{
    ValueTask<Failable<string>> Read(CancellationToken cancellation = default);
}