namespace Markwardt;

public interface IModel : IMultiDisposable, INotifyPropertyChanged, INotifyPropertyChanging;

public abstract class Model : ManagedAsyncDisposable, IModel;