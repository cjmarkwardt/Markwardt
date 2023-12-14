namespace Markwardt;

public interface IServiceTag
{
    IServiceDescription? Default { get; }
}

public static class ServiceTag
{
    public static bool GetDefault(object? target, [NotNullWhen(true)] out IServiceDescription? description)
    {
        description = null;

        if (target is IServiceTag tag)
        {
            description = tag.Default;
        }
        else if (target is Type tagType && tagType.IsAssignableTo(typeof(IServiceTag)))
        {
            description = ((IServiceTag)Activator.CreateInstance(tagType).NotNull()).Default;
        }

        return description != null;
    }

    public static IServiceDescription? GetDefault(object? target)
        => GetDefault(target, out IServiceDescription? description) ? description : null;
}