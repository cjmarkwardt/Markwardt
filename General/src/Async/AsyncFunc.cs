namespace Markwardt;

public delegate ValueTask<TResult> AsyncFunc<TResult>();
public delegate ValueTask<TResult> AsyncFunc<in T1, TResult>(T1 arg1);
public delegate ValueTask<TResult> AsyncFunc<in T1, in T2, TResult>(T1 arg1, T2 arg2);
public delegate ValueTask<TResult> AsyncFunc<in T1, in T2, in T3, TResult>(T1 arg1, T2 arg2, T3 arg3);
public delegate ValueTask<TResult> AsyncFunc<in T1, in T2, in T3, in T4, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
public delegate ValueTask<TResult> AsyncFunc<in T1, in T2, in T3, in T4, in T5, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
public delegate ValueTask<TResult> AsyncFunc<in T1, in T2, in T3, in T4, in T5, in T6, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);