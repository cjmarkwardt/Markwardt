namespace Markwardt;

public delegate ValueTask<Failable> AsyncAction(CancellationToken cancellation = default);
public delegate ValueTask<Failable> AsyncAction<in T1>(T1 arg1, CancellationToken cancellation = default);
public delegate ValueTask<Failable> AsyncAction<in T1, in T2>(T1 arg1, T2 arg2, CancellationToken cancellation = default);
public delegate ValueTask<Failable> AsyncAction<in T1, in T2, in T3>(T1 arg1, T2 arg2, T3 arg3, CancellationToken cancellation = default);
public delegate ValueTask<Failable> AsyncAction<in T1, in T2, in T3, in T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, CancellationToken cancellation = default);
public delegate ValueTask<Failable> AsyncAction<in T1, in T2, in T3, in T4, in T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, CancellationToken cancellation = default);
public delegate ValueTask<Failable> AsyncAction<in T1, in T2, in T3, in T4, in T5, in T6>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, CancellationToken cancellation = default);