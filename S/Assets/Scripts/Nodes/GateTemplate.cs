using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public enum NodeType
{
    Input, Output, Split, AND, NOT, ComplexGate, MultibitInput, MultibitOutput
}

//This class requires AppSaveData.gateTemplates
[Serializable]
public class GateTemplate
{
    public NodeType NodeType;
    public int bitCount;//tylko jesli NodeType => MultibitController
    //public bool isControlled;//tylko jesli NodeType => (In|Out)putNode
    public int inCnt, outCnt;
    public string defaultName;
    public int N;//, M;//number of verticies(nodes) and edges
    public RenderProperties renderProperties;
    public int[] TemplateIDsForEachNode;
    public ValueTuple<float, float>[] PositionsForEachNode;
    public int templateId;

    //(Source_Node_ID, OutID), (Destination_Node_Id, InID)
    public Pair<Pair<int, int>, Pair<int, int>>[] edges;
    //(List of controlled nodes, position)
    public List<ValueTuple<ValueTuple<float, float>, List<int>>> inputControllers, outputControllers;
    public Node BuildNodeFromTemplate(bool hidden = false, Vector2? where = null)
    {
        Node node;
        if (NodeType != NodeType.ComplexGate)
        {
            node = NodeType switch
            {
                NodeType.Input => new InputNode(hidden),// #### tutaj dodac wiecej typow inputu
                NodeType.Output => new OutputNode(hidden),
                NodeType.Split => new Split(hidden),
                NodeType.AND => new GateAND(hidden),
                NodeType.NOT => new GateNOT(hidden),
                NodeType.MultibitInput => new MultibitControllerInput(bitCount, hidden),
                NodeType.MultibitOutput => new MultibitControllerOutput(bitCount, hidden),
                _ => throw new Exception("unrecognized or not properly used gate type"),
            };
        }
        else
        {
            var gate = new GateComplex(inCnt, outCnt, defaultName, hidden, templateId);
            //tu sie dzieje główna rekurencyjna część:
            Node[] IDtoNode = new Node[N];
            for (int i = 0; i < N; i++)
            {
                //[TODO]: add checks for size
                int id = TemplateIDsForEachNode[i];
                GateTemplate template = AppSaveData.GetTemplate(id);
                IDtoNode[i] = template.BuildNodeFromTemplate(true); //lessgo
                                                                //TODO: save internal input/output sockets

                if (template.NodeType == NodeType.Input)
                {
                    var inputNode = (InputNode)IDtoNode[i];
                    gate.internalIns.Add(inputNode);
                }
                else if (template.NodeType == NodeType.Output)
                {
                    var outputNode = (OutputNode)IDtoNode[i];
                    gate.internalOuts.Add(outputNode);

                }
            }

            /* ----- controllers ----- */
            if(inputControllers != null)
            {

                foreach (var (pos, cinputs) in inputControllers)
                {
                    List<InputNode> controlledInputNodes = new();
                    foreach (var c in cinputs)
                    {
                        Debug.Assert(Helper.InRange(c, 0, IDtoNode.Length));
                        Debug.Assert(IDtoNode[c] is InputNode);
                        controlledInputNodes.Add(IDtoNode[c] as InputNode);
                    }
                    //create this at specified position,
                    //controlled nodes already exist
                    new MultibitControllerInput(controlledInputNodes, true)
                    {
                        Position = pos.ToVector2()
                    };
                }
            }

            if(outputControllers != null)
            {
                foreach (var (pos, coutputs) in outputControllers)
                {
                    List<OutputNode> controlledOutputNodes = new();
                    foreach (var c in coutputs)
                    {
                        Debug.Assert(Helper.InRange(c, 0, IDtoNode.Length));
                        Debug.Assert(IDtoNode[c] is OutputNode);
                        controlledOutputNodes.Add(IDtoNode[c] as OutputNode);
                    }
                    new MultibitControllerOutput(controlledOutputNodes, true)
                    {
                        Position = pos.ToVector2()
                    };
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

        if (where != null)
            node.Position = (Vector2)where;

        return node;
    }
}