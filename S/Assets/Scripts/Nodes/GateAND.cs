public class GateAND : Node
{
    public GateAND(bool hidden = false) : base(2, 1, "AND", hidden)
    {

    }
    public override void Calculate()
    {
        outVals[0] = inVals[0] && inVals[1];
    }
}
