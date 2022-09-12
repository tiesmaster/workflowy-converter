using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;

using TextCopy;

var workflowyBackupFilename = args[0];
var targetId = args.Length > 1 ? Guid.Parse(args[1]) : Guid.Empty;

var inputFile = File.OpenRead(workflowyBackupFilename);

var rootNode = JsonSerializer.Deserialize<WorkflowyNode>(inputFile);

if (targetId != Guid.Empty)
{
    rootNode = rootNode.GetNodeBydId(targetId);
}

var ms = new MemoryStream();
var writer = new StreamWriter(ms);

using var xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings
{
    Indent = true,
});

var root = new RootNode(rootNode);

root.WriteTo(xmlWriter);

xmlWriter.Flush();

var s = Encoding.UTF8.GetString(ms.ToArray());

ClipboardService.SetText(s);

public record RootNode(WorkflowyNode Node)
{
    public void WriteTo(XmlWriter writer)
    {
        writer.WriteStartElement("opml");
        writer.WriteAttributeString("version", "2.0");

        writer.WriteStartElement("body");

        Node.WriteTo(writer);

        writer.WriteEndElement();

        writer.WriteEndElement();
    }
}

public class WorkflowyNode
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("nm")]
    public string Todo { get; set; }

    [JsonPropertyName("ch")]
    public List<WorkflowyNode> Children { get; set; }

    [JsonPropertyName("cp")]
    public int? Completed { get; set; }

    public WorkflowyNode GetNodeBydId(Guid targetId)
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
}