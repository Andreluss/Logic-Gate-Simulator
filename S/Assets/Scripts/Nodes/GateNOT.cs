public class GateNOT : Node
{
    public GateNOT(bool hidden = false) : base(1, 1, "NOT", hidden)
    {

    }
    public override void Calculate()
    {
        outVals[0] = !inVals[0];
    }
}
