namespace Markwardt;

public abstract partial class GodotStartInitiator<TStarter> : GodotInitiator
    where TStarter : IStarter
{
    protected override sealed async ValueTask Start(IServiceResolver services)
    {
        TStarter starter = await services.Create<TStarter>();
        await starter.RouteLogsToTop(services);
        await starter.Start();
    }
}