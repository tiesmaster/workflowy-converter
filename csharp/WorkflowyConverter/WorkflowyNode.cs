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
        => GetNodeBydIdCore(targetId);

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