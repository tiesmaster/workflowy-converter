using System.Xml;

namespace Tiesmaster.Workflowy.Converter;

public class ToOpmlVisitor : Visitor
{
    private readonly XmlWriter _writer;

    public ToOpmlVisitor(XmlWriter writer)
    {
        _writer = writer;
    }

    public override void Visit(WorkflowyNode node)
    {
        _writer.WriteStartElement("outline");
        if (node.Completed.HasValue)
        {
            _writer.WriteAttributeString("_complete", "true");
        }

        _writer.WriteAttributeString("text", node.Todo);
        base.Visit(node);

        _writer.WriteEndElement();
    }
}