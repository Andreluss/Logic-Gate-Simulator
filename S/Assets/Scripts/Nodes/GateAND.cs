﻿public class GateAND : Node
{
    public GateAND(bool hidden = false) : base(2, 1, "AND", hidden)
    {

    }
    
    //Overrides
    public override int GetTemplateID()
    {
        return 3;
    }
    public override void Calculate()
    {
        outVals[0] = inVals[0] && inVals[1];
    }
}