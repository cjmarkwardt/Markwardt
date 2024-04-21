namespace Markwardt;

public interface IConnector<TMessage>
    where TMessage : notnull
{
    ValueTask<Failable<IConnection<TMessage>>> Connect(CancellationToken cancellation = default);
}

public static class ConnectorExtensions
{
    public static IConnector<TMessage> ConvertMessages<TSource, TMessage>(this IConnector<TSource> connector, ITwoWayConverter<TMessage, TSource> converter)
        where TSource : notnull
        where TMessage : notnull
        => new Connector<TSource, TMessage>(connector, converter);
}

public abstract record Connector<TMessage> : IConnector<TMessage>
    where TMessage : notnull
{
    public async ValueTask<Failable<IConnection<TMessage>>> Connect(CancellationToken cancellation = default)
    {
        Failable<IConnection<TMessage>> tryCreateConnection = await CreateConnection(cancellation);
        if (tryCreateConnection.Exception is not null)
        {
            return tryCreateConnection.Exception;
        }

        IConnection<TMessage> connection = tryCreateConnection.Result;

        Failable tryInitialization = await connection.Initialization;
        if (tryInitialization.Exception is not null)
        {
            await connection.DisposeAsync();
            return tryInitialization.Exception;
        }

        return connection.AsFailable();
    }

    protected abstract ValueTask<Failable<IConnection<TMessage>>> CreateConnection(CancellationToken cancellation = default);
}

public class Connector<TSource, TMessage>(IConnector<TSource> connector, ITwoWayConverter<TMessage, TSource> converter) : IConnector<TMessage>
    where TSource : notnull
    where TMessage : notnull
{
    public async ValueTask<Failable<IConnection<TMessage>>> Connect(CancellationToken cancellation = default)
    {
        Failable<IConnection<TSource>> tryConnect = await connector.Connect(cancellation);
        if (tryConnect.Exception is not null)
        {
            return tryConnect.Exception;
        }

        return tryConnect.Result.ConvertMessages(converter).AsFailable();
    }
}