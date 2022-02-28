public class GateNOT : Gate
{
    public GateNOT(bool hidden) : base(1, 1, "NOT", hidden)
    {

    }

    //Overrides
    public override int GetTemplateID()
    {
        return 9;
    }
    public override void Calculate()
    {
        outVals[0] = !inVals[0];
        base.Calculate();
    }
    //protected override void CreateRenderer()
    //{

    //}
    //protected override void DestroyRenderer()
    //{

    //}
}
