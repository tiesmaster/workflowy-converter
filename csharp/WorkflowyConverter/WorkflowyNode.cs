using System.Text.Json.Serialization;

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

    public int Level { get; set; }

    public int DescendentsAndSelfCount => Children?.Sum(c => c.DescendentsAndSelfCount) + 1 ?? 1;

    public WorkflowyNode? GetNodeBydId(Guid targetId)
    {
        var visitor = new NodeSearchingVisitor(targetId);
        visitor.Visit(this);

        return visitor.Node;
    }

    public OpmlDocument ToOpmlDocument()
        => new(this);

    public void Accept(Visitor visitor)
    {
        if (Children is not null)
        {
            foreach (var child in Children)
            {
                visitor.Visit(child);
            }
        }
    }
}