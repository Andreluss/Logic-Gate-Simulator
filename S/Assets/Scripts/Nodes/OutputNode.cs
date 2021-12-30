public class OutputNode : Node
{
    public OutputNode(bool hidden) : base(1, 0, "Output", hidden)
    {
    }
    //Overrides
    public override int GetTemplateID()
    {
        return 1;
    }
    protected override void CreateRenderer()
    {

    }
    protected override void DestroyRenderer()
    {

    }

    public bool GetValue() => inVals[0];
}
