namespace Markwardt;

public static class AsyncStateFunc
{
    public static AsyncStateFunc<TState, TResult> Create<TState, TResult>(AsyncFunc<TState> initialize, AsyncFunc<TState, TResult> invoke)
        => new(initialize, invoke);
        
    public static AsyncStateFunc<TState, T1, TResult> Create<TState, T1, TResult>(AsyncFunc<TState> initialize, AsyncFunc<TState, T1, TResult> invoke)
        => new(initialize, invoke);
        
    public static AsyncStateFunc<TState, T1, T2, TResult> Create<TState, T1, T2, TResult>(AsyncFunc<TState> initialize, AsyncFunc<TState, T1, T2, TResult> invoke)
        => new(initialize, invoke);
        
    public static AsyncStateFunc<TState, T1, T2, T3, TResult> Create<TState, T1, T2, T3, TResult>(AsyncFunc<TState> initialize, AsyncFunc<TState, T1, T2, T3, TResult> invoke)
        => new(initialize, invoke);
        
    public static AsyncStateFunc<TState, T1, T2, T3, T4, TResult> Create<TState, T1, T2, T3, T4, TResult>(AsyncFunc<TState> initialize, AsyncFunc<TState, T1, T2, T3, T4, TResult> invoke)
        => new(initialize, invoke);
        
    public static AsyncStateFunc<TState, T1, T2, T3, T4, T5, TResult> Create<TState, T1, T2, T3, T4, T5, TResult>(AsyncFunc<TState> initialize, AsyncFunc<TState, T1, T2, T3, T4, T5, TResult> invoke)
        => new(initialize, invoke);
}

public class AsyncStateFunc<TState, TResult>(AsyncFunc<TState> initialize, AsyncFunc<TState, TResult> invoke) : BaseAsyncStateAction<TState>(initialize)
{
    public static implicit operator AsyncFunc<TResult>(AsyncStateFunc<TState, TResult> action)
        => action.Invoke;

    public async ValueTask<TResult> Invoke()
    {
        await Initialize();
        return await invoke(State);
    }
}

public class AsyncStateFunc<TState, T1, TResult>(AsyncFunc<TState> initialize, AsyncFunc<TState, T1, TResult> invoke) : BaseAsyncStateAction<TState>(initialize)
{
    public static implicit operator AsyncFunc<T1, TResult>(AsyncStateFunc<TState, T1, TResult> action)
        => action.Invoke;

    public async ValueTask<TResult> Invoke(T1 arg1)
    {
        await Initialize();
        return await invoke(State, arg1);
    }
}

public class AsyncStateFunc<TState, T1, T2, TResult>(AsyncFunc<TState> initialize, AsyncFunc<TState, T1, T2, TResult> invoke) : BaseAsyncStateAction<TState>(initialize)
{
    public static implicit operator AsyncFunc<T1, T2, TResult>(AsyncStateFunc<TState, T1, T2, TResult> action)
        => action.Invoke;

    public async ValueTask<TResult> Invoke(T1 arg1, T2 arg2)
    {
        await Initialize();
        return await invoke(State, arg1, arg2);
    }
}

public class AsyncStateFunc<TState, T1, T2, T3, TResult>(AsyncFunc<TState> initialize, AsyncFunc<TState, T1, T2, T3, TResult> invoke) : BaseAsyncStateAction<TState>(initialize)
{
    public static implicit operator AsyncFunc<T1, T2, T3, TResult>(AsyncStateFunc<TState, T1, T2, T3, TResult> action)
        => action.Invoke;

    public async ValueTask<TResult> Invoke(T1 arg1, T2 arg2, T3 arg3)
    {
        await Initialize();
        return await invoke(State, arg1, arg2, arg3);
    }
}

public class AsyncStateFunc<TState, T1, T2, T3, T4, TResult>(AsyncFunc<TState> initialize, AsyncFunc<TState, T1, T2, T3, T4, TResult> invoke) : BaseAsyncStateAction<TState>(initialize)
{
    public static implicit operator AsyncFunc<T1, T2, T3, T4, TResult>(AsyncStateFunc<TState, T1, T2, T3, T4, TResult> action)
        => action.Invoke;

    public async ValueTask<TResult> Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        await Initialize();
        return await invoke(State, arg1, arg2, arg3, arg4);
    }
}

public class AsyncStateFunc<TState, T1, T2, T3, T4, T5, TResult>(AsyncFunc<TState> initialize, AsyncFunc<TState, T1, T2, T3, T4, T5, TResult> invoke) : BaseAsyncStateAction<TState>(initialize)
{
    public static implicit operator AsyncFunc<T1, T2, T3, T4, T5, TResult>(AsyncStateFunc<TState, T1, T2, T3, T4, T5, TResult> action)
        => action.Invoke;

    public async ValueTask<TResult> Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
    {
        await Initialize();
        return await invoke(State, arg1, arg2, arg3, arg4, arg5);
    }
}