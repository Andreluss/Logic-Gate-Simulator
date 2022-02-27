using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Split : Node
{
    public Split(bool hidden) : base(1, 1, "Edge Split", hidden)
    {
        
    }
    public SplitRenderer renderer;
    //Overrides
    public override int GetTemplateID()
    {
        return 8;
    }
    public override void Calculate()
    {
        outVals[0] = inVals[0];
    }
    protected override void CreateRenderer()
    {
        renderer = SplitRenderer.Make(this);
    }
    protected override void DestroyRenderer()
    {
        Object.Destroy(renderer.transform.parent.gameObject);
    }
    public override NodeRenderer GetRenderer()
    {
        return renderer;
    }
}
