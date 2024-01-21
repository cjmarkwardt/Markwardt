namespace Markwardt;

public static class StateAction
{
    public static StateAction<TState> Create<TState>(Func<TState> initialize, Action<TState> invoke)
        => new(initialize, invoke);
        
    public static StateAction<TState, T1> Create<TState, T1>(Func<TState> initialize, Action<TState, T1> invoke)
        => new(initialize, invoke);
        
    public static StateAction<TState, T1, T2> Create<TState, T1, T2>(Func<TState> initialize, Action<TState, T1, T2> invoke)
        => new(initialize, invoke);
        
    public static StateAction<TState, T1, T2, T3> Create<TState, T1, T2, T3>(Func<TState> initialize, Action<TState, T1, T2, T3> invoke)
        => new(initialize, invoke);
        
    public static StateAction<TState, T1, T2, T3, T4> Create<TState, T1, T2, T3, T4>(Func<TState> initialize, Action<TState, T1, T2, T3, T4> invoke)
        => new(initialize, invoke);
        
    public static StateAction<TState, T1, T2, T3, T4, T5> Create<TState, T1, T2, T3, T4, T5>(Func<TState> initialize, Action<TState, T1, T2, T3, T4, T5> invoke)
        => new(initialize, invoke);
}

public abstract class BaseStateAction<TState>(Func<TState> initialize)
{
    protected TState State { get; private set; } = default!;

    public bool IsInitialized { get; private set; }

    protected void Initialize()
    {
        if (!IsInitialized)
        {
            State = initialize();
            IsInitialized = true;
        }
    }
}

public class StateAction<TState>(Func<TState> initialize, Action<TState> invoke) : BaseStateAction<TState>(initialize)
{
    public static implicit operator Action(StateAction<TState> action)
        => action.Invoke;

    public void Invoke()
    {
        Initialize();
        invoke(State);
    }
}

public class StateAction<TState, T1>(Func<TState> initialize, Action<TState, T1> invoke) : BaseStateAction<TState>(initialize)
{
    public static implicit operator Action<T1>(StateAction<TState, T1> action)
        => action.Invoke;

    public void Invoke(T1 arg1)
    {
        Initialize();
        invoke(State, arg1);
    }
}

public class StateAction<TState, T1, T2>(Func<TState> initialize, Action<TState, T1, T2> invoke) : BaseStateAction<TState>(initialize)
{
    public static implicit operator Action<T1, T2>(StateAction<TState, T1, T2> action)
        => action.Invoke;

    public void Invoke(T1 arg1, T2 arg2)
    {
        Initialize();
        invoke(State, arg1, arg2);
    }
}

public class StateAction<TState, T1, T2, T3>(Func<TState> initialize, Action<TState, T1, T2, T3> invoke) : BaseStateAction<TState>(initialize)
{
    public static implicit operator Action<T1, T2, T3>(StateAction<TState, T1, T2, T3> action)
        => action.Invoke;

    public void Invoke(T1 arg1, T2 arg2, T3 arg3)
    {
        Initialize();
        invoke(State, arg1, arg2, arg3);
    }
}

public class StateAction<TState, T1, T2, T3, T4>(Func<TState> initialize, Action<TState, T1, T2, T3, T4> invoke) : BaseStateAction<TState>(initialize)
{
    public static implicit operator Action<T1, T2, T3, T4>(StateAction<TState, T1, T2, T3, T4> action)
        => action.Invoke;

    public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        Initialize();
        invoke(State, arg1, arg2, arg3, arg4);
    }
}

public class StateAction<TState, T1, T2, T3, T4, T5>(Func<TState> initialize, Action<TState, T1, T2, T3, T4, T5> invoke) : BaseStateAction<TState>(initialize)
{
    public static implicit operator Action<T1, T2, T3, T4, T5>(StateAction<TState, T1, T2, T3, T4, T5> action)
        => action.Invoke;

    public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
    {
        Initialize();
        invoke(State, arg1, arg2, arg3, arg4, arg5);
    }
}