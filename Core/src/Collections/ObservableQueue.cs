namespace Markwardt;

public interface IObservableQueue<T> : IObservableInsertionQueue<T>, IObservableConsumptionQueue<T>
    where T : notnull
{
    new int Count { get; }
}

public class ObservableQueue<T> : ObservableReadOnlyList<T>, IObservableQueue<T>
    where T : notnull
{
    public ObservableQueue(ISourceList<T> source)
        : base(source)
    {
        this.source = source;
        
        Connect().OnItemRemoved(OnRemove).Subscribe().DisposeWith(this);
    }

    public ObservableQueue()
        : this(new SourceList<T>()) { }

    public ObservableQueue(IEnumerable<T> items)
        : this()
        => Enqueue(items);

    private readonly ISourceList<T> source;
    
    public ItemDisposal ItemDisposal { get; set; } = ItemDisposal.None;

    public void Enqueue(IEnumerable<T> items)
        => source.AddRange(items);

    public IEnumerable<T> Dequeue(int count)
    {
        IEnumerable<T> dequeued = this.Take(Math.Min(count, Count)).ToList();
        source.RemoveMany(dequeued);
        return dequeued;
    }

    protected override void OnPrepareDisposal()
    {
        base.OnPrepareDisposal();

        if (ItemDisposal is ItemDisposal.OnDisposal || ItemDisposal is ItemDisposal.Full)
        {
            this.ForEach(x => x.DisposeWith(this));
        }
    }

    private void OnRemove(T item)
    {
        if (ItemDisposal is ItemDisposal.OnRemoval || ItemDisposal is ItemDisposal.Full)
        {
            this.DisposeInBackground(item);
        }
    }
}