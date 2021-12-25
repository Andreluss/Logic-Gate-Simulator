using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Dependencies:
// - AppSaveData
public class NodeManager //: Singleton<NodeManager>
{
    static HashSet<Node> nodes;

    public static void CreateNode(GateTemplate template, Vector2? where = null)
    {
        Node node = template.BuildNodeFromTemplate();
        node.Position = (Vector2)where;
        node.Hidden = false;

        nodes.Add(node);
    }
    
    public static GateTemplate SaveAllAsTemplate()
    {
        GateTemplate template = new();
        template.N = nodes.Count;
        template.TemplateIDsForEachNode = new int[template.N];
        Dictionary<Node, int> ID = new();
        int k = 0;
        foreach (var node in nodes)
        {
            template.TemplateIDsForEachNode[k] = node.GetTemplateID();
            ID[node] = k++;
        }
        List<Pair<Pair<int, int>, Pair<int, int>>> edges = new();

        template.edges = edges.ToArray();

        foreach (var node in nodes)
        {
        }
        return template;
    }

    public static GateTemplate SaveNodesAsTemplateALPHA(List<Node> nodes)
    {
        int newTemplateID = AppSaveData.GateTemplates.Length;
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
            //foreach (var outList in node.outs)
            //{
            //    foreach (var edge in outList)
            //    {
            //        Node to = edge.Item1; inIdx = edge.Item2;//reused var
            //        if(ID.ContainsKey(to))
            //        {

            //        }
            //        else
            //        {

            //        }
            //    }
            //}

        }
        GateTemplate template = new();
        return template;
    }
    
}
