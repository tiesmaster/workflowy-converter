using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;

namespace Tiesmaster.Workflowy.Converter;

public class WorkflowyNode
{
    [JsonPropertyName("id")]
    public Guid Id { get; init; }

    [JsonPropertyName("nm")]
    public string Todo { get; init; } = null!;

    [JsonPropertyName("ch")]
    public IEnumerable<WorkflowyNode>? Children { get; init; }

    [JsonPropertyName("cp")]
    public int? Completed { get; init; }

    public static WorkflowyNode ReadFrom(Stream stream)
        => JsonSerializer.Deserialize<WorkflowyNode>(stream)!;

    public WorkflowyNode GetNodeBydId(Guid targetId)
        => GetNodeBydIdCore(targetId)!;

    public OpmlDocument ToOpmlDocument()
        => new(this);

    public void WriteTo(XmlWriter writer)
    {
        writer.WriteStartElement("outline");
        if (Completed.HasValue)
        {
            writer.WriteAttributeString("_complete", "true");
        }

        writer.WriteAttributeString("text", Todo);

        if (Children is not null)
        {
            foreach (var child in Children)
            {
                child.WriteTo(writer);
            }
        }

        writer.WriteEndElement();
    }

    private WorkflowyNode? GetNodeBydIdCore(Guid targetId)
    {
        if (Id == targetId)
        {
            return this;
        }

        if (Children is not null)
        {
            foreach (var child in Children)
            {
                var targetNode = child.GetNodeBydId(targetId);
                if (targetNode is not null)
                {
                    return targetNode;
                }
            }
        }

        return default;
    }
}