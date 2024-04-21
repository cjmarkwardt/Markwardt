namespace Markwardt;

public class Injector<T>(T target)
    where T : IInjectable, IExtendedDisposable
{
    private bool isInitialized;

    public void Ready()
        => target.RunInBackground(async cancellation =>
        { 
            target.SetName(target.GetType().Name);
            await GlobalServices.Inject(this);
            await target.OnReady(cancellation);
            isInitialized = true;
        });

    public void Process(double delta)
    {
        if (isInitialized)
        {
            target.OnProcess(delta);
        }
    }
}