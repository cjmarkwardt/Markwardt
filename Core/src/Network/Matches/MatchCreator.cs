namespace Markwardt;

public interface IMatchCreator
{
    ValueTask<Failable<IMatchManager>> CreateMatch(CancellationToken cancellation = default);
}