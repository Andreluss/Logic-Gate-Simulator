using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//Dependencies:
// - AppSaveData
public static class NodeManager //: Singleton<NodeManager>
{
    static HashSet<Node> nodes = new HashSet<Node>();
    static HashSet<InputNode> inputNodes = new();
    static HashSet<OutputNode> outputNodes = new();
    public static Node CreateNode(GateTemplate template, Vector2? where = null)
    {
        Node node = template.BuildNodeFromTemplate();
        node.Hidden = false;
        if(where != null) 
            node.Position = (Vector2)where;

        nodes.Add(node);
        if (node is InputNode)
        {
            inputNodes.Add(node as InputNode); // hmm
        }
        else if (node is OutputNode)
        {
            outputNodes.Add((OutputNode)node); // hmm
        }
        return node;
    }

    public static void DeleteNode(Node node)
    {
        nodes.Remove(node);
        if(node is InputNode)
        {
            inputNodes.Remove(node as InputNode);
        }
        else if(node is OutputNode)
        {
            outputNodes.Remove(node as OutputNode);
        }

        node.Destroy();

        Debug.Log($"Node {node} deleted");//msdlkjl

        CalculateAll();
        //throw new NotImplementedException();
    }
    
    public static void Connect(Node A, int outIdx, Node B, int inIdx)
    {
        // ### Assuming the B.ins[inIdx] is free ###
        A.ConnectTo(outIdx, B, inIdx);
        // some extra actions itp. itd. 
        // update renderers and shit
    }
    public static void Disconnect(Node A, int outIdx, Node B, int inIdx)
    {
        A.DisconnectWith(outIdx, B, inIdx);
    }

    private static void CalculateAll()
    {
        NodeSearch.RunSearchAndCalculateAllNodes(inputNodes, outputNodes);
    }

    public static void ClearAll()
    {
        throw new NotImplementedException();
    }
    public static GateTemplate SaveAllAsTemplate(string newName) // TODO: ### jeszcze color i size !!!
    {
        GateTemplate template = new();
        template.defaultName = newName;
        //template.edges 
        template.inCnt = 0;
        template.N = nodes.Count;
        template.NodeType = NodeType.ComplexGate;
        template.outCnt = 0;
        //template.renderProperties ??????
        //template.templateId = AppSaveData.GateTemplates.Length; <-- this will be assigned internally 
        template.TemplateIDsForEachNode = new int[template.N];

        Dictionary<Node, int> ID = new();
        int k = 0;
        foreach (var node in nodes)
        {
            template.TemplateIDsForEachNode[k] = node.GetTemplateID();
            ID[node] = k++;
            if(node is InputNode)
            {
                template.inCnt++;
            }
            else if (node is OutputNode) {
                template.outCnt++;
            }
        }

        List<Pair<Pair<int, int>, Pair<int, int>>> edges = new();
        foreach (var from in nodes)
        {
            int outIdx = 0;
            foreach (var outList in from.outs)
            {
                foreach (var edge in outList)
                {
                    Node to = edge.Item1; int inIdx = edge.Item2;//reused var
                    edges.Add(new Pair<Pair<int, int>, Pair<int, int>>(new Pair<int, int>(ID[from], outIdx),
                                                                       new Pair<int, int>(ID[to], inIdx)));
                }
                outIdx++;
            }
        }
        template.edges = edges.ToArray();

        AppSaveData.AddTemplate(template);

        return template;
    }

    public static GateTemplate SaveNodesAsTemplateALPHA(List<Node> nodes, string templateName)
    {
        Dictionary<Node, int> ID = new Dictionary<Node, int>(); 
        List<Pair<Pair<int, int>, Pair<int, int>>> edges = new();
        List<int> TemplateIDsForEachNode = new();
        int k = 0;
        foreach (var node in nodes)
        {
            ID[node] = k++;
            TemplateIDsForEachNode.Add(node.GetTemplateID());
        }

        //int[] degIn = new int[k];//stopien wejsciowy wierzcholka

        Dictionary<Pair<Node, int>, int> internalIns = new();
        int l = k;

        foreach (var node in nodes)
        {
            foreach (var inEdge in node.ins)
            {
                Node from = inEdge.st; int outIdx = inEdge.nd;
                if(!ID.ContainsKey(inEdge.st) && !internalIns.ContainsKey(inEdge))
                {
                    internalIns.Add(inEdge, l++);
                    TemplateIDsForEachNode.Add(AppSaveData.InputTemplateID);
                }
            }
            int inIdx = 0;
            foreach (var inEdge in node.ins)
            {
                Node from = inEdge.st; int outIdx = inEdge.nd;
                int fromIdx;
                if (!ID.ContainsKey(from))
                {
                    fromIdx = internalIns[inEdge];
                    outIdx = 0;
                }
                else
                {
                    fromIdx = ID[from];
                }

                edges.Add(new Pair<Pair<int, int>, Pair<int, int>>(new Pair<int, int>(fromIdx, outIdx),
                                                                   new Pair<int, int>(ID[node], inIdx)));
                inIdx++;
            }

        }
        GateTemplate template = new();
        return template;
    }
    
}

public static class NodeSearch
{
    private static int CurrentSearchId = 0;
    public static void RunSearchAndCalculateAllNodes(IEnumerable<InputNode> inputs, IEnumerable<OutputNode> outputs)
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

                    //Updatujemy liczbe obliczonych inputów w s¹siednim nodzie
                    if (B.lastSearchId != CurrentSearchId)
                    {
                        B.lastSearchId = CurrentSearchId;
                        B.processedInCurrentSearch = 0;
                        B.inVals.Fill(false); //by default everything is 0(disconn.)
                    }
                    B.processedInCurrentSearch++;
                    //no i ten nowy input
                    B.inVals[inIdx] = value;

                    //now it's time to check if it's READY
                    if (B.processedInCurrentSearch == B.totalInputEdgesCount)
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