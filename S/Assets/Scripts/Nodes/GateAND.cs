﻿public class GateAND : Gate
{
    public GateAND(bool hidden) : base(2, 1, "AND", hidden)
    {

    }
    
    //Overrides
    public override int GetTemplateID()
    {
        return 10;
    }
    public override void Calculate()
    {
        outVals[0] = inVals[0] && inVals[1];
        base.Calculate();
    }
}
