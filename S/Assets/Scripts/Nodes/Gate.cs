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


public class Gate : Node
{
    public List<InputNode> internalIns;
    public List<OutputNode> internalOuts;

    public Gate(int inputCount, int outputCount, string name, bool hidden)
         : base(inputCount, outputCount, name, hidden)
    {
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

public static class NodeSearch
{
    private static int CurrentSearchId = 0;
    public static void RunSearchAndCalculateAllNodes(List<InputNode> inputs, List<OutputNode> outputs)
    {  
        Queue<Node> queue = new Queue<Node>();
        foreach (var inputNode in inputs)
        {
            queue.Enqueue(inputNode);
        }
        while (queue.Count > 0)
        {
            var A = queue.Dequeue();
            //assuming the A.inVals is calculated:
            A.Calculate();

            for (int outIdx = 0; outIdx < A.outs.Length; outIdx++)
            {
                bool value = A.outVals[outIdx];
                foreach (var edge in A.outs[outIdx])
                {
                    var B = edge.Item1;
                    var inIdx = edge.Item2;

                    //Updatujemy liczbe obliczonych inputów w sąsiednim nodzie
                    if(B.lastSearchId != CurrentSearchId)
                    {
                        B.lastSearchId = CurrentSearchId;
                        B.processedInCurrentSearch = 0;
                        B.inVals.Fill(false); //by default everything is 0(disconn.)
                    }
                    B.processedInCurrentSearch++;
                    //no i ten nowy input
                    B.inVals[inIdx] = value;
                    
                    //now it's time to check if it's READY
                    if(B.processedInCurrentSearch == B.totalInputEdgesCount)
                    {
                        //invariant (all inVals calculated) is maintained
                        queue.Enqueue(B);
                    }
                }
            }

        }

        //zeby nastepnym razem inputy sie tez zerowaly
        CurrentSearchId += 1;
    }
}