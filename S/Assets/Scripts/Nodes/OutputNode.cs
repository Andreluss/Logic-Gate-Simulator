public class OutputNode : Node
{
    public OutputNode(bool hidden = false) : base(1, 0, "Output", hidden)
    {
    }
    //Overrides
    public override int GetTemplateID()
    {
        return 1;
    }

    public bool GetValue() => outVals[0];
}
