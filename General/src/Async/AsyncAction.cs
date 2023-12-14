namespace Markwardt;

public delegate ValueTask AsyncAction();
public delegate ValueTask AsyncAction<T1>(T1 arg1);
public delegate ValueTask AsyncAction<T1, T2>(T1 arg1, T2 arg2);
public delegate ValueTask AsyncAction<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3);
public delegate ValueTask AsyncAction<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
public delegate ValueTask AsyncAction<T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
public delegate ValueTask AsyncAction<T1, T2, T3, T4, T5, T6>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);