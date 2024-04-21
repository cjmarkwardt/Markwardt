namespace Markwardt;

public record IpAddress(string Host, int Port)
{
    public override string ToString()
        => $"{Host}:{Port}";
}