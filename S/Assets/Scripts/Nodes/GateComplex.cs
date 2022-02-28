using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable] public class RenderProperties
{
    private readonly float[] color = new float[] { 0.1f, 0.8f, 0.0f, 1 };
    public float[] size = new float[] { 1, 2 };
    //public string name;

    public Color Color {
        get => new(color[0], color[1], color[2], color[3]);
        set 
        {
            color[0] = value.r;
            color[1] = value.g;
            color[2] = value.b;
            color[3] = value.a;
        }
    }
    public Vector2 Size
    {
        get => new(size[0], size[1]);
        set => size = new float[2]{ value.x, value.y };
    }
    public RenderProperties()
    {
    }
    public RenderProperties(Color color)
    {
        this.Color = color;
    }
    //...
}


//requires AppSaveData
public class GateComplex : Gate
{
    public List<InputNode> internalIns;
    public List<OutputNode> internalOuts;
    private int templateID;
    public GateComplex(int inputCount, int outputCount, string name, bool hidden, int templ)
         : base(inputCount, outputCount, name, hidden)
    {
        templateID = templ;
        internalIns = new();
        internalOuts = new();
    }

    public override int GetTemplateID()
    {
        return templateID;
    }

    public override void Calculate() // ? set value 
    {
        // Wymagania: wyliczona tablica inVals
        // Wynik: wyliczone outVals
        
        if(inVals.Length != internalIns.Count)
            throw new Exception("invals and internalIns have incompatible sizes");
        for (int i = 0; i < inVals.Length; i++)
        {
            internalIns[i].SetValue(inVals[i]); 
        }
        
        NodeSearch.RunSearchAndCalculateAllNodes(internalIns, null);

        if(outVals.Length != internalOuts.Count) 
            throw new Exception("invals and internalOuts have different sizes");
        for (int i = 0; i < outVals.Length; i++)
        {
            outVals[i] = internalOuts[i].GetValue();
        }

        base.Calculate();

    }
}