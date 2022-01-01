using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : Node
{
    public Gate(int inputCount, int outputCount, string name, bool hidden) 
         : base(inputCount, outputCount, name, hidden)
    {
    }

    public GateRenderer renderer;
    protected override void CreateRenderer()
    {
        Debug.Log("yeah nigggaaaa");
        renderer = GateRenderer.Make(this);
    }
    protected override void DestroyRenderer()
    {
        Object.Destroy(renderer.transform.parent);
        Debug.Log($"nigggaaaa gaterend = {renderer}");
    }
    public override NodeRenderer GetRenderer()
    {
        return renderer;
    }
}
