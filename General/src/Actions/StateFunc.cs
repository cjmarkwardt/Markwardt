namespace Markwardt;

public static class StateFunc
{
    public static StateFunc<TState, TResult> Create<TState, TResult>(Func<TState> initialize, Func<TState, TResult> invoke)
        => new(initialize, invoke);
        
    public static StateFunc<TState, T1, TResult> Create<TState, T1, TResult>(Func<TState> initialize, Func<TState, T1, TResult> invoke)
        => new(initialize, invoke);
        
    public static StateFunc<TState, T1, T2, TResult> Create<TState, T1, T2, TResult>(Func<TState> initialize, Func<TState, T1, T2, TResult> invoke)
        => new(initialize, invoke);
        
    public static StateFunc<TState, T1, T2, T3, TResult> Create<TState, T1, T2, T3, TResult>(Func<TState> initialize, Func<TState, T1, T2, T3, TResult> invoke)
        => new(initialize, invoke);
        
    public static StateFunc<TState, T1, T2, T3, T4, TResult> Create<TState, T1, T2, T3, T4, TResult>(Func<TState> initialize, Func<TState, T1, T2, T3, T4, TResult> invoke)
        => new(initialize, invoke);
        
    public static StateFunc<TState, T1, T2, T3, T4, T5, TResult> Create<TState, T1, T2, T3, T4, T5, TResult>(Func<TState> initialize, Func<TState, T1, T2, T3, T4, T5, TResult> invoke)
        => new(initialize, invoke);
}

public class StateFunc<TState, TResult>(Func<TState> initialize, Func<TState, TResult> invoke) : BaseStateAction<TState>(initialize)
{
    public static implicit operator Func<TResult>(StateFunc<TState, TResult> action)
        => action.Invoke;

    public TResult Invoke()
    {
        Initialize();
        return invoke(State);
    }
}

public class StateFunc<TState, T1, TResult>(Func<TState> initialize, Func<TState, T1, TResult> invoke) : BaseStateAction<TState>(initialize)
{
    public static implicit operator Func<T1, TResult>(StateFunc<TState, T1, TResult> action)
        => action.Invoke;

    public TResult Invoke(T1 arg1)
    {
        Initialize();
        return invoke(State, arg1);
    }
}

public class StateFunc<TState, T1, T2, TResult>(Func<TState> initialize, Func<TState, T1, T2, TResult> invoke) : BaseStateAction<TState>(initialize)
{
    public static implicit operator Func<T1, T2, TResult>(StateFunc<TState, T1, T2, TResult> action)
        => action.Invoke;

    public TResult Invoke(T1 arg1, T2 arg2)
    {
        Initialize();
        return invoke(State, arg1, arg2);
    }
}

public class StateFunc<TState, T1, T2, T3, TResult>(Func<TState> initialize, Func<TState, T1, T2, T3, TResult> invoke) : BaseStateAction<TState>(initialize)
{
    public static implicit operator Func<T1, T2, T3, TResult>(StateFunc<TState, T1, T2, T3, TResult> action)
        => action.Invoke;

    public TResult Invoke(T1 arg1, T2 arg2, T3 arg3)
    {
        Initialize();
        return invoke(State, arg1, arg2, arg3);
    }
}

public class StateFunc<TState, T1, T2, T3, T4, TResult>(Func<TState> initialize, Func<TState, T1, T2, T3, T4, TResult> invoke) : BaseStateAction<TState>(initialize)
{
    public static implicit operator Func<T1, T2, T3, T4, TResult>(StateFunc<TState, T1, T2, T3, T4, TResult> action)
        => action.Invoke;

    public TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        Initialize();
        return invoke(State, arg1, arg2, arg3, arg4);
    }
}

public class StateFunc<TState, T1, T2, T3, T4, T5, TResult>(Func<TState> initialize, Func<TState, T1, T2, T3, T4, T5, TResult> invoke) : BaseStateAction<TState>(initialize)
{
    public static implicit operator Func<T1, T2, T3, T4, T5, TResult>(StateFunc<TState, T1, T2, T3, T4, T5, TResult> action)
        => action.Invoke;

    public TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
    {
        Initialize();
        return invoke(State, arg1, arg2, arg3, arg4, arg5);
    }
}