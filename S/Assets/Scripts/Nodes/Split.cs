using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Split : Node
{
    public Split(bool hidden) : base(1, 1, "Edge Split", hidden)
    {
        
    }

    //Overrides
    public override int GetTemplateID()
    {
        return 2;
    }
    public override void Calculate()
    {
        outVals[0] = inVals[0];
    }
    protected override void CreateRenderer()
    {

    }
    protected override void DestroyRenderer()
    {

    }
}
