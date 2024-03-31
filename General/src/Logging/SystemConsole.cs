namespace Markwardt;

public class SystemConsole : IConsole
{
    public void Write(string message)
        => Console.WriteLine(message);

    public async ValueTask<Failable<string>> Read(CancellationToken cancellation = default)
        => await Abortable.Execute(() => Console.ReadLine() ?? string.Empty, cancellation);
}