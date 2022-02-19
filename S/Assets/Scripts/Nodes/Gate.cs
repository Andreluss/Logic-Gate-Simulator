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
        renderer = GateRenderer.Make(this);
    }
    protected override void DestroyRenderer()
    {
        Object.Destroy(renderer.transform.parent.gameObject);
        Debug.Log($"Gaterend {renderer} destroyed");
    }
    public override NodeRenderer GetRenderer()
    {
        return renderer;
    }
}
