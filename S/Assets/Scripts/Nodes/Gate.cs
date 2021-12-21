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

[Serializable] public enum NodeType
{
    Input, Output, AND, NOT, Split, ComplexGate
}

//This class requires AppSaveData.gateTemplates
[Serializable]
public class GateTemplate
{
    public NodeType NodeType;
    public int inCnt, outCnt;
    public string defaultName;
    public int N, M;//number of verticies(nodes) and edges
    public RenderProperties renderProperties;
    public int[] TemplateIDsForEachNode;
    public Pair<Pair<int, int>, Pair<int, int>>[] edges;
    //(Source_Node_ID, OutID), (Destination_Node_Id, InID)
    public Node BuildNodeFromTemplate(bool hidden = true, Vector2? where = null)
    {
        Node node;
        if(NodeType != NodeType.ComplexGate)
        {
            switch (NodeType)
            {
                case NodeType.Input:
                    node = new InputNode(hidden);
                    break;
                case NodeType.Output:
                    node = new OutputNode(hidden);
                    break;
                case NodeType.AND:
                    node = new GateAND(hidden);
                    break;
                case NodeType.NOT:
                    node = new GateNOT(hidden);
                    break;
                case NodeType.Split:
                    node = new Split(hidden);
                    break;
                default:
                    throw new Exception("unrecognized gate type");
            }
        }
        else
        {
            var gate = new Gate(inCnt, outCnt, defaultName, hidden);
            //tu sie dzieje główna rekurencyjna część:
            Node[] IDtoNode = new Node[N];
            for (int i = 0; i < N; i++)
            {
                //TODO: add cheks for size
                int id = TemplateIDsForEachNode[i];
                GateTemplate template = AppSaveData.GateTemplates[id];
                IDtoNode[i] = template.BuildNodeFromTemplate(); //lessgo
                //TODO: save internal input/output sockets
                
                if(template.NodeType == NodeType.Input)
                {
                    var inputNode = (InputNode)IDtoNode[i];
                    gate.internalIns.Add(inputNode);
                }
                else if(template.NodeType == NodeType.Output)
                {
                    var outputNode = (OutputNode)IDtoNode[i];
                    gate.internalOuts.Add(outputNode);

                }
            }

            foreach (var edge in edges)
            {
                //TODO: checks here as well
                Node A = IDtoNode[edge.st.st],
                     B = IDtoNode[edge.nd.st];
                int outIdx = edge.st.nd,
                    inpIdx = edge.nd.nd;

                A.ConnectTo(outIdx, B, inpIdx);
            }

            node = gate;
        }

        if(where != null)
            node.Position = (Vector2)where;

        return node;
    }
}

public class Gate : Node
{
    public List<InputNode> internalIns;
    public List<OutputNode> internalOuts;

    public Gate(int inputCount, int outputCount, string name, bool hidden)
         : base(inputCount, outputCount, name, hidden)
    {
    }

    public override void Calculate()
    {
        // Wymagania: wyliczona tablica inVals
        // Wynik: wyliczone outVals
        
        inVals
        NodeSearch.RunSearchAndCalculateAllNodes(internalIns, internalOuts);
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