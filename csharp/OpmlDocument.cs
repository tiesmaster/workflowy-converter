using System.Text;
using System.Xml;

namespace Tiesmaster.Workflowy.Converter;

public record OpmlDocument(WorkflowyNode Node)
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

    public override string ToString()
    {
        var ms = new MemoryStream();

        var writer = new StreamWriter(ms);

        using var xmlWriter = XmlWriter.Create(
            writer,
            new XmlWriterSettings
            {
                Indent = true
            });

        WriteTo(xmlWriter);

        xmlWriter.Flush();

        var s = Encoding.UTF8.GetString(ms.ToArray());

        return s;
    }
}