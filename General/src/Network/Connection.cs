namespace Markwardt;

public interface IConnection<TMessage> : IMultiDisposable
    where TMessage : notnull
{
    ConnectionState State { get; }
    IObservableConsumptionQueue<TMessage> Receives { get; }

    IObservableResult<Failable> Initialization { get; }
    IObservableResult<string> Disconnection { get; }

    void Send(TMessage message, NetworkDelivery delivery = NetworkDelivery.Full);
    void Disconnect(string? reason = null);
}

public static class ConnectionExtensions
{
    public static IConnection<TMessage> ConvertMessages<TSource, TMessage>(this IConnection<TSource> connection, ITwoWayConverter<TMessage, TSource> converter, bool disposeSource = true)
        where TSource : notnull
        where TMessage : notnull
        => new Connection<TSource, TMessage>(connection, converter, disposeSource);
}

public abstract class Connection<TMessage> : ExtendedDisposable, IConnection<TMessage>
    where TMessage : notnull
{
    private ConnectionState state;
    public ConnectionState State { get => state; set => this.RaiseAndSetIfChanged(ref state, value); }

    private readonly SourceResult<Failable> initialization = new();
    public IObservableResult<Failable> Initialization => initialization;

    private readonly SourceResult<string> disconnection = new();
    public IObservableResult<string> Disconnection => disconnection;

    private readonly ObservableQueue<TMessage> receives = new();
    public IObservableConsumptionQueue<TMessage> Receives => receives;

    public void Send(TMessage message, NetworkDelivery delivery = NetworkDelivery.Full)
    {
        if (State is not ConnectionState.Disconnected)
        {
            ExecuteSend(message, delivery);
        }
    }

    public void Disconnect(string? reason = null)
    {
        if (State is not ConnectionState.Disconnected)
        {
            bool isConnecting = State is ConnectionState.Connecting;

            State = ConnectionState.Disconnected;
            ExecuteDisconnect();
            disconnection.SetResult(reason ?? "Manual disconnect");

            if (isConnecting)
            {
                initialization.SetResult(Failable.Fail(reason ?? "Failed to connect"));
            }
        }
    }

    protected abstract void ExecuteSend(TMessage message, NetworkDelivery delivery);
    protected abstract void ExecuteDisconnect();

    protected void Connect()
    {
        if (State is ConnectionState.Connecting)
        {
            State = ConnectionState.Connected;
            initialization.SetResult(Failable.Success());
        }
    }

    protected void Receive(TMessage data)
    {
        if (State is not ConnectionState.Disconnected)
        {
            receives.Enqueue(data);
        }
    }

    protected override void OnPrepareDisposal()
    {
        base.OnPrepareDisposal();

        receives.DisposeWith(this);
    }
}

public class Connection<TSource, T> : ExtendedDisposable, IConnection<T>
    where TSource : notnull
    where T : notnull
{
    public Connection(IConnection<TSource> source, ITwoWayConverter<T, TSource> converter, bool disposeSource = true)
    {
        this.source = source;
        this.converter = converter;

        if (disposeSource)
        {
            source.DisposeWith(this);
        }

        state = source.WhenAnyValue(x => x.State).ToProperty(this, x => x.State).DisposeWith(this);
        source.Receives.Consume().Subscribe(x => receives.Enqueue(converter.ConvertBack(x))).DisposeWith(this);
    }

    private readonly IConnection<TSource> source;
    private readonly ITwoWayConverter<T, TSource> converter;

    private readonly ObservableQueue<T> receives = new() { ItemDisposal = ItemDisposal.OnDisposal };
    public IObservableConsumptionQueue<T> Receives => receives;

    public IObservableResult<Failable> Initialization => source.Initialization;
    public IObservableResult<string> Disconnection => source.Disconnection;

    private readonly ObservableAsPropertyHelper<ConnectionState> state;
    public ConnectionState State => state.Value;

    public void Send(T message, NetworkDelivery delivery = NetworkDelivery.Full)
        => source.Send(converter.Convert(message), delivery);

    public void Disconnect(string? reason = null)
        => source.Disconnect(reason);
}