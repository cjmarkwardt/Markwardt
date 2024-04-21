namespace Markwardt;

public class CreatorTag<TCreator> : SourceTag<TCreator>
    where TCreator : IServiceCreator
{
    protected override async ValueTask<object> Create(TCreator source)
        => await source.Create();
}