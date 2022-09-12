using System.Xml;

namespace Tiesmaster.Workflowy.Converter;

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