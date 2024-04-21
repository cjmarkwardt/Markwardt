namespace Markwardt;

public static class ExceptionExtensions
{
    public static string GetRecursiveMessage(this Exception exception)
    {
        StringBuilder message = new(exception.Message);
        Exception? current = exception.InnerException;
        while (current != null)
        {
            message.Append($"; {current.Message}");
            current = current.InnerException;
        }

        return message.ToString();
    }
}