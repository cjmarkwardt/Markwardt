namespace Markwardt;

public class JsonDataConverter : IExportConverter<JsonNode?>
{
    public static string TypeKey => "$type";
    public static string ValuesKey => "$values";

    public JsonNode? Convert(IExportNode? export)
    {
        if (export == null)
        {
            return null;
        }
        
        if (export.Type == null && export.Properties.Count == 0)
        {
            if (export.Values.Count == 1 && export.Values[0] is IExportField valueField)
            {
                return valueField.Content;
            }
            else if (export.Values.Count > 1 && export.Values.All(x => x == null || x is IExportField))
            {
                return new JsonArray(export.Values.Select(ConvertValue).ToArray());
            }
        }

        JsonObject target = [];

        if (export.Type != null)
        {
            target[TypeKey] = export.Type;
        }

        if (export.Values.Count > 0)
        {
            target[ValuesKey] = new JsonArray(export.Values.Select(ConvertValue).ToArray());
        }

        foreach (KeyValuePair<string, IExportNode?> property in export.Properties)
        {
            target[property.Key] = Convert(property.Value);
        }

        return target;
    }

    public IExportNode? Deconvert(JsonNode? target)
    {
        if (target == null)
        {
            return null;
        }
        
        ExportNode node = new();

        if (target is JsonValue value)
        {
            node.Values.Add(DeconvertValue(value));
        }
        else if (target is JsonArray array)
        {
            foreach (JsonNode? item in array)
            {
                node.Values.Add(DeconvertValue(item));
            }
        }
        else if (target is JsonObject obj)
        {
            if (obj.ContainsKey(TypeKey))
            {
                node.Type = (string)obj[TypeKey]!;
            }

            if (obj.ContainsKey(ValuesKey))
            {
                foreach (JsonNode? item in obj[ValuesKey]!.AsArray())
                {
                    node.Values.Add(DeconvertValue(item));
                }
            }

            foreach (KeyValuePair<string, JsonNode?> property in obj)
            {
                node.Properties[property.Key] = Deconvert(property.Value);
            }
        }
        else
        {
            throw new InvalidOperationException();
        }

        return node;
    }

    private JsonNode? ConvertValue(IExportValue? export)
    {
        if (export == null)
        {
            return null;
        }
        else if (export is IExportField field)
        {
            return field.Content;
        }
        else if (export is IExportNode node)
        {
            return Convert(node);
        }
        else
        {
            throw new InvalidOperationException();
        }
    }

    private IExportValue? DeconvertValue(JsonNode? target)
    {
        if (target == null)
        {
            return null;
        }
        else if (target is JsonValue value)
        {
            return new ExportField((string)value!);
        }
        else
        {
            return Deconvert(target);
        }
    }
}
