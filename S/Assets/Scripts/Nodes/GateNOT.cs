public class GateNOT : Gate
{
    public GateNOT(bool hidden) : base(1, 1, "NOT", hidden)
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
    //protected override void CreateRenderer()
    //{
        
    //}
    //protected override void DestroyRenderer()
    //{

    //}
}
