using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable] public class RenderProperties
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


//requires AppSaveData
public class Gate : Node
{
    public List<InputNode> internalIns;
    public List<OutputNode> internalOuts;
    private int templateID;
    public Gate(int inputCount, int outputCount, string name, bool hidden, int templ)
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
        
        NodeSearch.RunSearchAndCalculateAllNodes(internalIns, internalOuts);

        if(outVals.Length != internalOuts.Count) 
            throw new Exception("invals and internalOuts have different sizes");
        for (int i = 0; i < outVals.Length; i++)
        {
            outVals[i] = internalOuts[i].GetValue();
        }
    }
}