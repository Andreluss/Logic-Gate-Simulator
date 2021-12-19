using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class RenderProperties
{
    private float[] color = new float[] { 0.1f, 0.8f, 0.0f, 1 };
    public float[] size = new float[] { 1, 2 };

    public Color Color {
        get => new Color(color[0], color[1], color[2], color[3]);
        set 
        {
            color[0] = value.r;
            color[1] = value.g;
            color[2] = value.b;
            color[3] = value.a;
        }
    } 
    //...
}
[Serializable]
public enum NodeType
{
    Input, Output, AND, NOT, Split, ComplexGate
}

[Serializable]
public class GateTemplate
{
    public int inCnt, outCnt;
    public int N, M;//number of verticies(nodes) and edges
    RenderProperties renderProperties = new();
    public Pair<NodeType, int>[] nodes;
    public Pair<Pair<int, int>, Pair<int, int>> edges;
}

public static class AppSaveData
{
    private static GateTemplate[] gateTemplates;
    //workaround:
    public static GateTemplate[] GateTemplates {
        get
        {
            if(gateTemplates == null) 
                gateTemplates = Helpers.LoadClass<GateTemplate[]>(Application.dataPath + "/GateTemplates.bin");
            return gateTemplates;
        }
        set => gateTemplates = value; 
    }
    //TODO: load this at the start
}

public class Gate : Node
{
    GateTemplate gateTemplate;// template
    public Gate(int inCnt, int outCnt, string name): base(inCnt, outCnt, name)
    {
        Build();
    }
    public override void Build()
    {
        
    }
}