namespace Tiesmaster.Workflowy.Converter;

public class NodeSearchingVisitor : Visitor
{
    private readonly Guid _targetId;

    public NodeSearchingVisitor(Guid targetId)
    {
        _targetId = targetId;
    }

    public WorkflowyNode? Node { get; private set; }

    public override void Visit(WorkflowyNode node)
    {
        if (node.Id == _targetId)
        {
            Node = node;
        }
        else
        {
            base.Visit(node);
        }
    }
}