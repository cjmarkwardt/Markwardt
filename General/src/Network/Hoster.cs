namespace Markwardt;

public interface IHoster<TMessage>
    where TMessage : notnull
{
    ValueTask<Failable<IHost<TMessage>>> Host(bool isListening = true, CancellationToken cancellation = default);
}

public static class HosterExtensions
{
    public static IHoster<TMessage> ConvertMessages<TSource, TMessage>(this IHoster<TSource> hoster, ITwoWayConverter<TMessage, TSource> converter)
        where TSource : notnull
        where TMessage : notnull
        => new Hoster<TSource, TMessage>(hoster, converter);
}

public abstract record Hoster<TMessage> : IHoster<TMessage>
    where TMessage : notnull
{
    public async ValueTask<Failable<IHost<TMessage>>> Host(bool isListening = true, CancellationToken cancellation = default)
    {
        Failable<IHost<TMessage>> tryCreateHost = await CreateHost(isListening, cancellation);
        if (tryCreateHost.Exception is not null)
        {
            return tryCreateHost.Exception;
        }

        IHost<TMessage> host = tryCreateHost.Result;

        Failable tryInitialization = await host.Initialization;
        if (tryInitialization.Exception is not null)
        {
            await host.DisposeAsync();
            return tryInitialization.Exception;
        }

        return host.AsFailable();
    }

    protected abstract ValueTask<Failable<IHost<TMessage>>> CreateHost(bool isListening = true, CancellationToken cancellation = default);
}

public class Hoster<TSource, TMessage>(IHoster<TSource> hoster, ITwoWayConverter<TMessage, TSource> converter) : IHoster<TMessage>
    where TSource : notnull
    where TMessage : notnull
{
    public async ValueTask<Failable<IHost<TMessage>>> Host(bool isListening = true, CancellationToken cancellation = default)
    {
        Failable<IHost<TSource>> tryConnect = await hoster.Host(isListening, cancellation);
        if (tryConnect.Exception is not null)
        {
            return tryConnect.Exception;
        }

        return tryConnect.Result.ConvertMessages(converter).AsFailable();
    }
}