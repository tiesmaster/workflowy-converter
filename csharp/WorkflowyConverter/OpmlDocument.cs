using System.Text;
using System.Text.Json;
using System.Xml;

namespace Tiesmaster.Workflowy.Converter;

public record OpmlDocument(HashSet<WorkflowyNode> RootNodes)
{
    public OpmlDocument(params WorkflowyNode[] rootNodes)
        : this(rootNodes.ToHashSet())
    {
    }

    public static async Task<OpmlDocument> ReadFromAsync(Stream stream)
    {
        var rootNodes = await JsonSerializer.DeserializeAsync<WorkflowyNode[]>(stream);

        var visitor = new LevelUpdatingVisitor();
        foreach (var rootNode in rootNodes!)
        {
            visitor.Visit(rootNode);
        }

        return new(rootNodes);
    }

    public OpmlDocument? GetOpmlDocumentById(Guid targetId)
    {
        foreach (var rootNodes in RootNodes)
        {
            var node = rootNodes.GetNodeBydId(targetId);
            if (node is not null)
            {
                return node.ToOpmlDocument();
            }
        }

        return default;
    }

    public override string ToString()
    {
        using var ms = new MemoryStream();
        WriteTo(ms);

        return Encoding.UTF8.GetString(ms.ToArray());
    }

    public void WriteTo(Stream stream)
    {
        using var writer = new StreamWriter(stream);
        WriteTo(writer);
    }

    public void WriteTo(TextWriter textWriter)
    {
        using var xmlWriter = XmlWriter.Create(
            textWriter,
            new XmlWriterSettings { Indent = true });

        WriteTo(xmlWriter);
    }

    public void WriteTo(XmlWriter writer)
    {
        writer.WriteStartElement("opml");
        writer.WriteAttributeString("version", "2.0");

        writer.WriteStartElement("body");

        var visitor = new ToOpmlVisitor(writer);
        foreach (var rootNode in RootNodes)
        {
            visitor.Visit(rootNode);
        }

        writer.WriteEndElement();

        writer.WriteEndElement();
    }
}