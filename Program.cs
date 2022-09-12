using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

using TextCopy;

var workflowyBackupFilename = Util.ReadLine<string>("What is the location of the Workflowy backup file?");
var targetId = Util.ReadLine<Guid?> ("What is the ID of the node you want to convert to OPML (leave empty for the root node)?");

var inputFile = File.OpenRead(workflowyBackupFilename);

var rootNode = JsonSerializer.Deserialize<WorkflowyNode>(inputFile);

if (targetId.HasValue)
{
    rootNode = rootNode.GetNodeBydId(targetId.Value);
}

var ms = new MemoryStream();
var xmlSerializer = new XmlSerializer(typeof(WorkflowyNode));
var writer = new StreamWriter(ms);

var xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings
{
    Indent = true,
});

var root = new RootNode(rootNode);

root.WriteTo(xmlWriter);

xmlWriter.Flush();

var s = Encoding.UTF8.GetString(ms.ToArray());
s.Dump();

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