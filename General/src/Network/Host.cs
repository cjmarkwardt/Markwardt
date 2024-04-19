namespace Markwardt;

public interface IHost<TMessage> : IMultiDisposable
    where TMessage : notnull
{
    IObservableConsumptionQueue<IConnection<TMessage>> Receives { get; }

    bool IsListening { get; set; }
    
    IObservableResult<Failable> Initialization { get; }
}

public static class HostExtensions
{
    public static IHost<TMessage> ConvertMessages<TSource, TMessage>(this IHost<TSource> host, ITwoWayConverter<TMessage, TSource> converter, bool disposeSource = true)
        where TSource : notnull
        where TMessage : notnull
        => new Host<TSource, TMessage>(host, converter, disposeSource);
}

public abstract class Host<TMessage>(bool isListening) : ExtendedDisposable, IHost<TMessage>
    where TMessage : notnull
{
    private readonly ObservableQueue<IConnection<TMessage>> receives = new() { ItemDisposal = ItemDisposal.OnDisposal };
    public IObservableConsumptionQueue<IConnection<TMessage>> Receives => receives;

    private readonly SourceResult<Failable> initialization = new();
    public IObservableResult<Failable> Initialization => initialization;

    public bool IsListening { get; set; } = isListening;

    protected override void OnPrepareDisposal()
    {
        base.OnPrepareDisposal();

        receives.DisposeWith(this);
    }

    protected void Receive(IConnection<TMessage> connection)
    {
        if (IsListening)
        {
            receives.Enqueue(connection);
        }
        else
        {
            this.DisposeInBackground(connection);
        }
    }

    protected void Initialize(Failable? result = null)
    {
        result ??= Failable.Success();

        if (result.IsSuccess())
        {
            OnInitialized();
        }

        initialization.SetResult(result);
    }

    protected virtual void OnInitialized() { }
}

public class Host<TSource, T> : ExtendedDisposable, IHost<T>
    where TSource : notnull
    where T : notnull
{
    public Host(IHost<TSource> source, ITwoWayConverter<T, TSource> converter, bool disposeSource = true)
    {
        if (disposeSource)
        {
            source.DisposeWith(this);
        }

        this.source = source;

        source.Receives.Consume().Subscribe(x => receives.Enqueue(x.ConvertMessages(converter))).DisposeWith(this);
    }

    private readonly IHost<TSource> source;

    private readonly ObservableQueue<IConnection<T>> receives = new() { ItemDisposal = ItemDisposal.OnDisposal };
    public IObservableConsumptionQueue<IConnection<T>> Receives => receives;

    public bool IsListening { get => source.IsListening; set => source.IsListening = value; }

    public IObservableResult<Failable> Initialization => source.Initialization;
}