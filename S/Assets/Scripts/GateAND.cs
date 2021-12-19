public class GateAND : Node
{
    public GateAND() : base(2, 1, "AND")
    {
    }

    public override void Calculate()
    {
        outVals[0] = inVals[0] && inVals[1];
    }
}
