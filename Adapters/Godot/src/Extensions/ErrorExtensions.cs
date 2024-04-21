namespace Markwardt;

public static class ErrorExtensions
{
    public static void Verify(this Error error)
    {
        if (error is not Error.Ok)
        {
            throw new InvalidOperationException(error.ToString());
        }
    }
}