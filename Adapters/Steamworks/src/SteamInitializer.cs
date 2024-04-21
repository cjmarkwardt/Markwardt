namespace Markwardt;

[Singleton<SteamInitalizer>]
public interface ISteamInitializer : IMultiDisposable
{
    bool IsInitialized { get; }

    Failable Initialize();
}

public class SteamInitalizer : ExtendedDisposable, ISteamInitializer
{
    public bool IsInitialized { get; private set; }

    public Failable Initialize()
    {
        if (IsInitialized)
        {
            return Failable.Success();
        }

        if (!DllCheck.Test() || !Packsize.Test())
        {
            return Failable.Fail("Invalid version of Steam API");
        }

        if (!SteamAPI.Init())
        {
            return Failable.Fail("Failed to initialize Steam");
        }

        IsInitialized = true;

        this.LoopInBackground(TimeSpan.FromMilliseconds(25), _ =>
        {
            SteamAPI.RunCallbacks();
            return ValueTask.CompletedTask;
        });

        return Failable.Success();
    }

    protected override void OnSharedDisposal()
    {
        base.OnSharedDisposal();

        if (IsInitialized)
        {
            SteamAPI.Shutdown();

            IsInitialized = false;
        }
    }
}