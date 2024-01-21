namespace Markwardt;

public interface IExportConverter<T>
{
    T Convert(IExportNode? export);
    IExportNode? Deconvert(T target);
}