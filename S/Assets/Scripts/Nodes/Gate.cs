using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : Node
{
    public Gate(int inputCount, int outputCount, string name, bool hidden) 
         : base(inputCount, outputCount, name, hidden)
    {
    }

    public GateRenderer gateRenderer;
    protected override void CreateRenderer()
    {
        Debug.Log("yeah nigggaaaa");
        gateRenderer = GateRenderer.Make(this);
    }
    protected override void DestroyRenderer()
    {
        Object.Destroy(gateRenderer.gameObject.transform.parent);
        Debug.Log($"nigggaaaa gaterend = {gateRenderer}");
    }
}
