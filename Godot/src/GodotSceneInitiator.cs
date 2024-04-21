namespace Markwardt;

public abstract partial class GodotSceneInitiator : GodotInitiator
{
    protected abstract string StartScene { get; }

    protected override sealed ValueTask Start(IServiceResolver services)
    {
        GetTree().SetScene(StartScene);
        return ValueTask.CompletedTask;
    }
}