namespace Tiesmaster.Workflowy.Converter;

public class Visitor
{
    public virtual void Visit(WorkflowyNode node)
    {
        node.Accept(this);
    }
}