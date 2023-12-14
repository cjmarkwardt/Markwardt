namespace Markwardt;

public interface IModel : INotifyPropertyChanged, INotifyPropertyChanging, IMultiDisposable;

public abstract class Model : ManagedAsyncDisposable, IModel
{
    private readonly ReactiveObject reactive = new();

    public event PropertyChangingEventHandler? PropertyChanging
    {
        add => reactive.PropertyChanging += value;
        remove => reactive.PropertyChanging -= value;
    }

    public event PropertyChangedEventHandler? PropertyChanged
    {
        add => reactive.PropertyChanged += value;
        remove => reactive.PropertyChanged -= value;
    }
}