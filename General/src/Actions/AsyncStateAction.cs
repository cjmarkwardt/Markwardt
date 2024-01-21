namespace Markwardt;

public static class AsyncStateAction
{
    public static AsyncStateAction<TState> Create<TState>(AsyncFunc<TState> initialize, AsyncAction<TState> invoke)
        => new(initialize, invoke);
        
    public static AsyncStateAction<TState, T1> Create<TState, T1>(AsyncFunc<TState> initialize, AsyncAction<TState, T1> invoke)
        => new(initialize, invoke);
        
    public static AsyncStateAction<TState, T1, T2> Create<TState, T1, T2>(AsyncFunc<TState> initialize, AsyncAction<TState, T1, T2> invoke)
        => new(initialize, invoke);
        
    public static AsyncStateAction<TState, T1, T2, T3> Create<TState, T1, T2, T3>(AsyncFunc<TState> initialize, AsyncAction<TState, T1, T2, T3> invoke)
        => new(initialize, invoke);
        
    public static AsyncStateAction<TState, T1, T2, T3, T4> Create<TState, T1, T2, T3, T4>(AsyncFunc<TState> initialize, AsyncAction<TState, T1, T2, T3, T4> invoke)
        => new(initialize, invoke);
        
    public static AsyncStateAction<TState, T1, T2, T3, T4, T5> Create<TState, T1, T2, T3, T4, T5>(AsyncFunc<TState> initialize, AsyncAction<TState, T1, T2, T3, T4, T5> invoke)
        => new(initialize, invoke);
}

public abstract class BaseAsyncStateAction<TState>(AsyncFunc<TState> initialize)
{
    protected TState State { get; private set; } = default!;

    public bool IsInitialized { get; private set; }

    protected async ValueTask Initialize()
    {
        if (!IsInitialized)
        {
            State = await initialize();
            IsInitialized = true;
        }
    }
}

public class AsyncStateAction<TState>(AsyncFunc<TState> initialize, AsyncAction<TState> invoke) : BaseAsyncStateAction<TState>(initialize)
{
    public static implicit operator AsyncAction(AsyncStateAction<TState> action)
        => action.Invoke;

    public async ValueTask Invoke()
    {
        await Initialize();
        await invoke(State);
    }
}

public class AsyncStateAction<TState, T1>(AsyncFunc<TState> initialize, AsyncAction<TState, T1> invoke) : BaseAsyncStateAction<TState>(initialize)
{
    public static implicit operator AsyncAction<T1>(AsyncStateAction<TState, T1> action)
        => action.Invoke;

    public async ValueTask Invoke(T1 arg1)
    {
        await Initialize();
        await invoke(State, arg1);
    }
}

public class AsyncStateAction<TState, T1, T2>(AsyncFunc<TState> initialize, AsyncAction<TState, T1, T2> invoke) : BaseAsyncStateAction<TState>(initialize)
{
    public static implicit operator AsyncAction<T1, T2>(AsyncStateAction<TState, T1, T2> action)
        => action.Invoke;

    public async ValueTask Invoke(T1 arg1, T2 arg2)
    {
        await Initialize();
        await invoke(State, arg1, arg2);
    }
}

public class AsyncStateAction<TState, T1, T2, T3>(AsyncFunc<TState> initialize, AsyncAction<TState, T1, T2, T3> invoke) : BaseAsyncStateAction<TState>(initialize)
{
    public static implicit operator AsyncAction<T1, T2, T3>(AsyncStateAction<TState, T1, T2, T3> action)
        => action.Invoke;

    public async ValueTask Invoke(T1 arg1, T2 arg2, T3 arg3)
    {
        await Initialize();
        await invoke(State, arg1, arg2, arg3);
    }
}

public class AsyncStateAction<TState, T1, T2, T3, T4>(AsyncFunc<TState> initialize, AsyncAction<TState, T1, T2, T3, T4> invoke) : BaseAsyncStateAction<TState>(initialize)
{
    public static implicit operator AsyncAction<T1, T2, T3, T4>(AsyncStateAction<TState, T1, T2, T3, T4> action)
        => action.Invoke;

    public async ValueTask Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        await Initialize();
        await invoke(State, arg1, arg2, arg3, arg4);
    }
}

public class AsyncStateAction<TState, T1, T2, T3, T4, T5>(AsyncFunc<TState> initialize, AsyncAction<TState, T1, T2, T3, T4, T5> invoke) : BaseAsyncStateAction<TState>(initialize)
{
    public static implicit operator AsyncAction<T1, T2, T3, T4, T5>(AsyncStateAction<TState, T1, T2, T3, T4, T5> action)
        => action.Invoke;

    public async ValueTask Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
    {
        await Initialize();
        await invoke(State, arg1, arg2, arg3, arg4, arg5);
    }
}