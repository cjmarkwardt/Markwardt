namespace Markwardt;

public static class InvokeExtensions
{
    public static void InvokeAll(this IEnumerable<Action> actions)
    {
        foreach (Action action in actions)
        {
            action();
        }
    }
    
    public static Task InvokeAll(this IEnumerable<AsyncAction> actions)
        => Task.WhenAll(actions.Select(action => action.Invoke().AsTask()));
}