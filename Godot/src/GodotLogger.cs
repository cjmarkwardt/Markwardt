namespace Markwardt.Godot;

public class GodotLogger : Logger
{
    protected override void OnLog(LogMessage message)
    {
        if (message.IsFailure)
        {
            GD.PushError(message.ToString());
        }
        else
        {
            GD.Print(message.ToString());
        }
    }
}