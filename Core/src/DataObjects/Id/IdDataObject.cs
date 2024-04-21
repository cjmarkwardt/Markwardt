namespace Markwardt;

public interface IIdDataObject : IDataObject
{
    string Id { get; }
}

public sealed class IdDataObject(IIdDataModel model, Action destroyClaim) : DynamicObject, IIdDataObject, IDataAccessor
{
    private IIdDataModel DataModel => model;

    private bool isDisposed;

    public event PropertyChangedEventHandler? PropertyChanged
    {
        add => DataModel.PropertyChanged += value;
        remove => DataModel.PropertyChanged -= value;
    }

    public string Id => model.Id;
    public bool IsDeleted => model.IsDeleted;

    public void SetIndex(string index)
        => model.SetIndex(index);

    public void Delete()
        => model.Delete();

    public Maybe<object?> GetProperty(string name)
        => model.GetProperty(name);

    public bool SetProperty(string name, object? value)
        => model.SetProperty(name, value);

    public bool Equals(IDataObject? other)
        => other is IdDataObject otherEntity && model.Id == otherEntity.DataModel.Id;

    public void Dispose()
    {
        if (!isDisposed)
        {
            isDisposed = true;
            destroyClaim();
        }
    }

    public override bool TryGetMember(GetMemberBinder binder, out object? result)
        => model.GetProperty(binder.Name).TryGetValue(out result);

    public override bool TrySetMember(SetMemberBinder binder, object? value)
        => model.SetProperty(binder.Name, value);
}