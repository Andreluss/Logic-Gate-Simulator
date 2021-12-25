using UnityEngine;
using System;

[Serializable]
public enum NodeType
{
    Input, Output, Split, AND, NOT, ComplexGate
}

//This class requires AppSaveData.gateTemplates
[Serializable]
public class GateTemplate
{
    public NodeType NodeType;
    public int inCnt, outCnt;
    public string defaultName;
    public int N;//, M;//number of verticies(nodes) and edges
    public RenderProperties renderProperties;
    public int[] TemplateIDsForEachNode;
    public int templateId;
    public Pair<Pair<int, int>, Pair<int, int>>[] edges;
    //(Source_Node_ID, OutID), (Destination_Node_Id, InID)
    public Node BuildNodeFromTemplate(bool hidden = true, Vector2? where = null)
    {
        Node node;
        if (NodeType != NodeType.ComplexGate)
        {
            switch (NodeType)
            {
                case NodeType.Input:
                    node = new InputNode(hidden); // #### tutaj dodac wiecej typow inputu
                    break;
                case NodeType.Output:
                    node = new OutputNode(hidden);
                    break;
                case NodeType.Split:
                    node = new Split(hidden);
                    break;
                case NodeType.AND:
                    node = new GateAND(hidden);
                    break;
                case NodeType.NOT:
                    node = new GateNOT(hidden);
                    break;
                default:
                    throw new Exception("unrecognized gate type");
            }
        }
        else
        {
            var gate = new Gate(inCnt, outCnt, defaultName, hidden, templateId);
            //tu sie dzieje główna rekurencyjna część:
            Node[] IDtoNode = new Node[N];
            for (int i = 0; i < N; i++)
            {
                //TODO: add cheks for size
                int id = TemplateIDsForEachNode[i];
                GateTemplate template = AppSaveData.GateTemplates[id];
                IDtoNode[i] = template.BuildNodeFromTemplate(); //lessgo
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