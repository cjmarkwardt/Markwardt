namespace Markwardt;

public interface IDataStore : IComponent
{
    [Factory<DataStore>]
    delegate IDataStore Factory(IFile file, ITransformer<DataObject> transformer);

    ValueTask<DataObject> Access();
    ValueTask Save();
    ValueTask Unload();
}

public class DataStore(IFile file, ITransformer<DataObject> transformer) : Component, IDataStore
{
    private readonly SequentialExecutor executor = new();

    private bool isLoaded;
    private DataObject root = new();

    public async ValueTask<DataObject> Access()
        => await executor.Execute(async () =>
        {
            if (!isLoaded)
            {
                Failable<DataObject> tryRead = await file.Read(transformer);
                if (tryRead.Exception != null)
                {
                    this.Error(tryRead.Exception);
                }
                else
                {
                    root = tryRead.Result;
                    isLoaded = true;
                }
            }

            return root;
        });

    public async ValueTask Save()
        => await executor.Execute(async () =>
        {
            if (isLoaded)
            {
                Failable tryOverwrite = await file.Overwrite(transformer, root);
                if (tryOverwrite.Exception != null)
                {
                    this.Error(tryOverwrite.Exception);
                }
            }
        });

    public async ValueTask Unload()
        => await executor.Execute(() =>
        {
            root = new();
            isLoaded = false;
        });

    protected override void PrepareDisposal()
    {
        base.PrepareDisposal();

        executor.DisposeWith(this);
    }
}