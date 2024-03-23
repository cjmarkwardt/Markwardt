namespace Markwardt.Godot;

public class GodotLogger : ComplexDisposable
{
    public override void Log(LogMessage report)
    {
        base.Log(report);

        if (report.Category == "Error")
        {
            GD.PushError(report.ShortenSource().ToString(DateTime.Now));
        }
        else
        {
            GD.Print(report.ShortenSource().ToString(DateTime.Now));
        }
    }
}