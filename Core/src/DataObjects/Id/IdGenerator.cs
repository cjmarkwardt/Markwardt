namespace Markwardt;

public interface IIdGenerator
{
    string Generate();
}

public class IdGenerator : IIdGenerator
{
    public string Generate()
        => Guid.NewGuid().ToString("N").ToUpper();
}