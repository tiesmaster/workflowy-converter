using System.Text;
using System.Xml;

namespace Tiesmaster.Workflowy.Converter;

public record OpmlDocument(WorkflowyNode Node)
{
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

        Node.WriteTo(writer);

        writer.WriteEndElement();

        writer.WriteEndElement();
    }
}