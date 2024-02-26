namespace Markwardt.Godot;

public class GodotLogger : Logger
{
    protected override bool PushLog(LogReport report)
    {
        if (report.Category == "Error")
        {
            GD.PushError(report.ShortenSource().ToString(DateTime.Now));
        }
        else
        {
            GD.Print(report.ShortenSource().ToString(DateTime.Now));
        }

        return true;
    }
}