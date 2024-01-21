namespace Markwardt;

public static class InitializeFlag
{
    private static readonly ConditionalWeakTable<object, Flag> flags = new();

    public static bool IsInitialized(object target)
        => flags.TryGetValue(target, out _);

    public static void SetInitialized(object target)
        => flags.AddOrUpdate(target, Flag.Instance);

    private sealed class Flag
    {
        public static Flag Instance { get; } = new();

        private Flag() { }
    }
}