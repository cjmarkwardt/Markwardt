namespace Markwardt;

public abstract partial class GodotStartInitiator<TStarter> : GodotInitiator
    where TStarter : IStarter
{
    protected override sealed async ValueTask Start(IServiceResolver services)
        => await (await services.Create<TStarter>()).Start();
}