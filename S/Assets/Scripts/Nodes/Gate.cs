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
    }
    protected override void DestroyRenderer()
    {
        Debug.Log("nigggaaaa");
    }
}
