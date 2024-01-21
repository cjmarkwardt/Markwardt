namespace Markwardt;

public interface IExportField : IExportValue
{
    string Content { get; }
}

public record ExportField(string Content) : IExportField;