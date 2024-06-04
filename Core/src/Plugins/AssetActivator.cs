namespace Markwardt;

[RoutedService<IAssetManager>]
public interface IAssetActivator
{
    ValueTask Activate(string id);
}