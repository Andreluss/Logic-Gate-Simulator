public class GateNOT : Node
{
    public GateNOT(bool hidden = false) : base(1, 1, "NOT", hidden)
    {

    }

    //Overrides
    public override int GetTemplateID()
    {
        return 4;
    }
    public override void Calculate()
    {
        outVals[0] = !inVals[0];
    }
}
