namespace Markwardt;

public interface IIdDataModel : IDataIndexable, IDataDeletable, INotifyPropertyChanged, IDataAccessor
{
    string Id { get; }
}

public class IdDataModel : IIdDataModel
{
    public static async ValueTask<IdDataModel> Setup(IIdDataNotifier notifier, IIdDataLoader loader, IIdDataLoader directLoader, ITypeNamer typeNamer, string id, Type type, bool isCreating, IReadOnlyDictionary<string, object?> properties)
    {
        IdDataModel model = new(notifier, typeNamer, id, type);

        foreach (PropertyInfo property in type.GetInterfaces().Prepend(type).SelectMany(x => x.GetProperties()).Where(x => x.DeclaringType != typeof(IDataIndexable) && x.DeclaringType != typeof(IDataDeletable)))
        {
            object? value;
            if (properties.TryGetValue(property.Name, out object? argument))
            {
                value = argument;
            }
            else if (property.TryGetCustomAttribute(out DataDefaultAttribute? defaultAttribute))
            {
                value = defaultAttribute.Value;
            }
            else if (property.PropertyType.HasGenericTypeDefinition(typeof(IDataLink<>)))
            {
                value = null;
            }
            else if (property.PropertyType.HasGenericTypeDefinition(typeof(IDataLinkSet<>)))
            {
                value = Enumerable.Empty<string>();
            }
            else
            {
                throw new InvalidOperationException($"Property {property.Name} in entity type {type.Name} has no argument or default");
            }

            if (property.PropertyType.HasGenericTypeDefinition(typeof(IDataLink<>)))
            {
                Type target = property.PropertyType.GetGenericArguments()[0];
                Type classType = typeof(IdDataLink<>).MakeGenericType(target);

                string? targetId;
                if (value is IIdDataLink link)
                {
                    targetId = link.TargetId;
                }
                else if (value is IIdDataObject entity)
                {
                    targetId = entity.Id;
                }
                else if (value is string rawId)
                {
                    targetId = rawId;
                }
                else if (value is null)
                {
                    targetId = null;
                }
                else
                {
                    throw new InvalidOperationException($"Invalid link type {value.GetType()}");
                }

                IIdDataUpdater? updater = null;
                if (property.TryGetCustomAttribute(out DataRelationshipAttribute? relationship))
                {
                    updater = new IdDataUpdater(id, relationship.PropertyName);
                }

                if (isCreating && targetId != null && updater != null)
                {
                    IDataObject? targetEntity = await directLoader.TryLoad(targetId);
                    if (targetEntity != null)
                    {
                        updater.AddTo(targetEntity);
                    }
                }

                value = Activator.CreateInstance(classType, loader, updater, targetId);
            }
            else if (property.PropertyType.HasGenericTypeDefinition(typeof(IDataLinkSet<>)))
            {
                Type classType = typeof(IdDataLinkSet<>).MakeGenericType(property.PropertyType.GetGenericArguments()[0]);

                IEnumerable<string> targetIds;
                if (value is IIdDataLinkSet linkSet)
                {
                    targetIds = linkSet.TargetIds;
                }
                else if (value is IIdDataObject entity)
                {
                    targetIds = Enumerable.Repeat(entity.Id, 1);
                }
                else if (value is string rawId)
                {
                    targetIds = Enumerable.Repeat(rawId, 1);
                }
                else if (value is IEnumerable enumerable)
                {
                    List<string> ids = [];
                    foreach (object? obj in enumerable)
                    {
                        if (obj is IIdDataObject enumeratedEntity)
                        {
                            ids.Add(enumeratedEntity.Id);
                        }
                        else if (obj is string enumeratedId)
                        {
                            ids.Add(enumeratedId);
                        }
                    }

                    targetIds = ids;
                }
                else if (value is null)
                {
                    targetIds = Enumerable.Empty<string>();
                }
                else
                {
                    throw new InvalidOperationException($"Invalid link set type {value.GetType()}");
                }

                IIdDataUpdater? updater = null;
                if (property.TryGetCustomAttribute(out DataRelationshipAttribute? relationship))
                {
                    updater = new IdDataUpdater(id, relationship.PropertyName);
                }

                if (isCreating && targetIds.Any() && updater != null)
                {
                    foreach (string targetId in targetIds)
                    {
                        IDataObject? targetEntity = await directLoader.TryLoad(targetId);
                        if (targetEntity != null)
                        {
                            updater.AddTo(targetEntity);
                        }
                    }
                }

                value = Activator.CreateInstance(classType, loader, updater, targetIds);
            }
            
            model.properties[property.Name] = value;
        }

        return model;
    }

    public static async ValueTask<IdDataModel> Create(IIdDataNotifier notifier, IIdDataLoader loader, IIdDataLoader directLoader, ITypeNamer typeNamer, string id, Type type, IReadOnlyDictionary<string, object?> arguments)
        => await Setup(notifier, loader, directLoader, typeNamer, id, type, true, arguments);

    public static async ValueTask<IdDataModel> Load(IIdDataNotifier notifier, IIdDataLoader loader, IIdDataLoader directLoader, ITypeNamer typeNamer, IdData data)
        => await Setup(notifier, loader, directLoader, typeNamer, data.Id, typeNamer.GetType(data.Type), false, data.Properties.AsReadOnly());

    private IdDataModel(IIdDataNotifier notifier, ITypeNamer typeNamer, string id, Type type)
    {
        this.notifier = notifier;
        this.typeNamer = typeNamer;
        Id = id;
        this.type = type;
    }

    private readonly IIdDataNotifier notifier;
    private readonly ITypeNamer typeNamer;
    private readonly Type type;
    private readonly Dictionary<string, object?> properties = [];

    private CancellationTokenSource? cancelExpiration;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Id { get; }

    public int Claims { get; private set; }
    public bool IsDeleted { get; private set; }

    public IDataObject CreateClaim()
    {
        cancelExpiration?.Cancel();
        cancelExpiration?.Dispose();
        cancelExpiration = null;

        Claims++;
        RaisePropertyChanged(nameof(Claims));
        return Impromptu.DynamicActLike(new IdDataObject(this, OnClaimDestroyed), type, typeof(IIdDataObject));
    }

    public void SetIndex(string index)
        => notifier.MarkIndexChanged(index, Id);

    public void Delete()
    {
        if (!IsDeleted)
        {
            IsDeleted = true;
            RaisePropertyChanged(nameof(IsDeleted));
            notifier.MarkEntityDeleted(Id);
        }
    }

    public Maybe<object?> GetProperty(string name)
        => properties.TryGetValue(name, out object? value) ? value.Maybe() : default;

    public bool SetProperty(string name, object? value)
    {
        if (properties.ContainsKey(name))
        {
            properties[name] = value;
            RaisePropertyChanged(name);
            return true;
        }

        return false;
    }

    public IdData Export()
        => new(Id, typeNamer.GetName(type), properties.Select(x =>
        {
            if (x.Value is IIdDataLink link)
            {
                return new KeyValuePair<string, object?>(x.Key, link.TargetId);
            }
            else if (x.Value is IIdDataLinkSet linkSet)
            {
                return new KeyValuePair<string, object?>(x.Key, linkSet.TargetIds.ToHashSet());
            }

            return x;
        }));

    private async void OnClaimDestroyed()
    {
        Claims--;
        RaisePropertyChanged(nameof(Claims));

        if (Claims == 0)
        {
            try
            {
                cancelExpiration = new();
                await Task.Delay(TimeSpan.FromMinutes(5), cancelExpiration.Token);
                cancelExpiration.Dispose();
                cancelExpiration = null;
            }
            catch (OperationCanceledException)
            {
                return;
            }

            notifier.MarkEntityExpired(Id);
        }
    }

    private void RaisePropertyChanged(string name)
        => PropertyChanged?.Invoke(this, new(name));
}