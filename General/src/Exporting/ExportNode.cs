namespace Markwardt;

public interface IExportNode : IExportValue
{
    string? Type { get; set; }

    IList<IExportValue?> Values { get; }
    IDictionary<string, IExportNode?> Properties { get; }
}

public class ExportNode : IExportNode
{
    public ExportNode() { }

    public ExportNode(string value)
        => Values.Add(new ExportField(value));

    public string? Type { get; set; }

    public IList<IExportValue?> Values { get; } = [];
    public IDictionary<string, IExportNode?> Properties { get; } = new Dictionary<string, IExportNode?>();
}