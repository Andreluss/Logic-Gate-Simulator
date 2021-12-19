using System;
using System.Collections;

[Serializable]
public class GateTemplate
{
    public int inCnt, outCnt; 
}

public class Gate : Node
{
    // template
    public Gate(int inCnt, int outCnt, string name): base(inCnt, outCnt, name)
    {
        Build();
    }
    public override void Build()
    {
        
    }
}