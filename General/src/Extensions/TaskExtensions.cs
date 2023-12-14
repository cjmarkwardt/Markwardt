namespace Markwardt;

public static class TaskExtensions
{
    private static readonly Dictionary<Type, Specifier> specifiers = [];
    private static readonly Dictionary<Type, Generalizer> generalizers = [];

    public static bool IsVoidTask(this Type type)
        => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task);

    public static ValueTask AsValueTask(this Task task)
        => new(task);

    public static ValueTask<TResult> AsValueTask<TResult>(this Task<TResult> task)
        => new(task);

    public static Type GetResultType(this Task task)
    {
        Type type = task.GetType();
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>))
        {
            return type.GetGenericArguments()[0];
        }
        else
        {
            return typeof(void);
        }
    }

    public static Type GetResultType(this ValueTask task)
    {
        Type type = task.GetType();
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ValueTask<>))
        {
            return type.GetGenericArguments()[0];
        }
        else
        {
            return typeof(void);
        }
    }

    public static async ValueTask<T?> Specify<T>(this ValueTask<object?> task)
        => (T?) await task;

    public static object Specify(this ValueTask<object?> task, Type resultType)
    {
        if (!specifiers.TryGetValue(resultType, out Specifier? specifier))
        {
            specifier = CreateSpecifier(resultType);
            specifiers.Add(resultType, specifier);
        }

        return specifier(task);
    }

    public static async ValueTask<object?> Generalize<T>(this ValueTask<T?> task)
        => await task;

    public static ValueTask<object?> Generalize(object task)
    {
        Type type = task.GetType();
        if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(ValueTask<>))
        {
            throw new InvalidOperationException();
        }

        Type resultType = type.GetGenericArguments()[0];

        if (!generalizers.TryGetValue(resultType, out Generalizer? generalizer))
        {
            generalizer = CreateGeneralizer(resultType);
            generalizers.Add(resultType, generalizer);
        }

        return generalizer(task);
    }

    private static Specifier CreateSpecifier(Type type)
    {
        ParameterExpression task = Expression.Parameter(typeof(ValueTask<object?>));
        return Expression.Lambda<Specifier>(Expression.Convert(Expression.Call(typeof(TaskExtensions), nameof(Specify), [type], task), typeof(object)), task).Compile();
    }

    private static Generalizer CreateGeneralizer(Type type)
    {
        ParameterExpression task = Expression.Parameter(typeof(object));
        return Expression.Lambda<Generalizer>(Expression.Call(typeof(TaskExtensions), nameof(Generalize), [type], Expression.Convert(task, typeof(ValueTask<>).MakeGenericType(type))), task).Compile();
    }

    private delegate object Specifier(ValueTask<object?> task);
    private delegate ValueTask<object?> Generalizer(object task);

    public static async Task<bool> TryDelay(TimeSpan delay, CancellationToken cancellation = default)
    {
        try
        {
            await Task.Delay(delay, cancellation);
            return true;
        }
        catch (OperationCanceledException)
        {
            return false;
        }
    }

    public static async void Fork(this Task task)
        => await task;

    public static void Fork(this Func<Task> task)
        => task().Fork();
}