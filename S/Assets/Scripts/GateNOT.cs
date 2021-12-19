public class GateNOT : Node
{
    public GateNOT(): base(1, 1, "NOT")
    {
    }

    public override void Calculate()
    {
        outVals[0] = !inVals[0];
    }
}
