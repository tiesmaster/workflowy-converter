namespace Tiesmaster.Workflowy.Converter;

public class LevelUpdatingVisitor : Visitor
{
    private int _level;

    public override void Visit(WorkflowyNode node)
    {
        node.Level = _level;

        _level++;
        base.Visit(node);
        _level--;
    }
}
