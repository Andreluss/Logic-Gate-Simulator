using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

//Dependencies:
// - AppSaveData
//[TODO] zmieniæ Nodemanager na Monobehaviour Singleton
public static class NodeManager //: Singleton<NodeManager>
{
    static readonly HashSet<Node> nodes = new();//widzialne nody
    static readonly HashSet<Node> controllers = new();//widzialne controllery
    static readonly HashSet<InputNode> inputNodes = new();
    static readonly HashSet<OutputNode> outputNodes = new();
    /// <summary>
    /// Funkcja, która tworzy i rejestruje w menedzerze nowy klocek z danego template'u
    /// </summary>
    /// <param name="template">template klocka, ktory chcemy utworzyc</param>
    /// <param name="where">null lub pozycja klocka</param>
    /// <returns></returns>
    public static Node CreateNode(GateTemplate template, Vector2? where = null)
    {
        Node node = template.BuildNodeFromTemplate();
        node.Hidden = false;
        if(where != null) 
            node.Position = (Vector2)where;

        RegisterNode(node);

        return node;
    }

    /// <summary>
    /// Funkcja umozliwiajaca zapisanie przez NodeManager <b>stworzonego ju¿</b> klocka
    /// </summary>
    /// <param name="node"></param>
    public static void RegisterNode(Node node)
    {
        if (node is not MultibitController)
            nodes.Add(node);
        else
            controllers.Add(node);

        if (node is InputNode)
        {
            inputNodes.Add(node as InputNode); // hmm
        }
        else if (node is OutputNode)
        {
            outputNodes.Add(node as OutputNode); // hmm
        }
        else if (node is MultibitControllerInput mcinput)
        {
            foreach (var inputNode in mcinput.Inputs)
            {
                nodes.Add(inputNode);
                inputNodes.Add(inputNode);
            }
        }
        else if (node is MultibitControllerOutput mco)
        {
            foreach (var outnode in mco.Outputs)
            {
                nodes.Add(outnode);
                outputNodes.Add(outnode);
            }
        }

        CalculateAll();
    }

    public static void DeleteNode(Node node)
    {
        if (node is not MultibitController)
            nodes.Remove(node);
        else
            controllers.Remove(node);

        if (node is InputNode)
        {
            inputNodes.Remove(node as InputNode);
        }
        else if(node is OutputNode)
        {
            outputNodes.Remove(node as OutputNode);
        }
        else if (node is MultibitControllerInput mcinput)
        {
            foreach (var inputNode in mcinput.Inputs)
            {
                nodes.Remove(inputNode);
                inputNodes.Remove(inputNode);
            }
        }
        else if (node is MultibitControllerOutput mco)
        {
            foreach (var outnode in mco.Outputs)
            {
                nodes.Remove(outnode);
                outputNodes.Remove(outnode);
            }
        }

        node.Destroy();

        Debug.Log($"Node {node} deleted");//msdlkjl

        CalculateAll();
    }
    
    public static void Connect(Node A, int outIdx, Node B, int inIdx)
    {
        // ### Assuming the B.ins[inIdx] is free ###
        // ??? [DESIGN] ???
        // UPDATE: jesli socket B.ins[inIdx] jest wolny, to nic nie robimy....
        //  ........ALBO usuwamy poprzedni¹ krawêdŸ i tworzymy t¹ now¹??? 
        
        A.ConnectTo(outIdx, B, inIdx);
        // some extra actions itp. itd. 
        // update renderers and shit
        
        CalculateAll();
    }
    public static void Disconnect(Node A, int outIdx, Node B, int inIdx)
    {
        A.DisconnectWith(outIdx, B, inIdx);

        CalculateAll();
    }

    public static void CalculateAll()
    {
        //[DESIGN] ?? ?? 
        //jako input mozna tez wzi¹æ wszystkie wiecho³ki ktore maja totalInputEdgesCount == 0

        //(A)
        //NodeSearch.RunSearchAndCalculateAllNodes(inputNodes);

        //(B)
        List<Node> readyNodes = new();
        foreach(var node in nodes)
        {
            if(node.totalInputEdgesCount == 0) 
                readyNodes.Add(node);
        }
        NodeSearch.RunSearchAndCalculateAllNodes(readyNodes, controllers);
    }

    private static bool descriptionsEnabled = true;
    public static void ToggleDestriptions()
    {
        descriptionsEnabled = !descriptionsEnabled;
        foreach (var node in nodes)
        {
            if(node is InputNode)
                (node as InputNode).GetRenderer().ShowDescription(descriptionsEnabled);
            else if(node is OutputNode)
                (node as OutputNode).GetRenderer().ShowDescription(descriptionsEnabled);
        }
    }

    public static void Flip(InputCollision inputCollision)
    {
        inputCollision.InputNode.FlipValue();
        CalculateAll();
    }

    public static void ClearAll()
    {
        throw new NotImplementedException();
    }
    private static GateTemplate GetTemplateFromAll()
    {
        GateTemplate template = new();
        //template.edges 
        template.inCnt = 0;
        template.N = nodes.Count;//controllers ?
        template.NodeType = NodeType.ComplexGate;
        template.outCnt = 0;
        //template.templateId = AppSaveData.GateTemplates.Length; <-- this will be assigned internally 
        template.TemplateIDsForEachNode = new int[template.N];
        template.PositionsForEachNode = new (float, float)[template.N];

        Dictionary<Node, int> ID = new();
        int k = 0;
        foreach (var node in nodes)//[TODO] check perf
        {
            template.TemplateIDsForEachNode[k] = node.GetTemplateID();
            template.PositionsForEachNode[k] = node.Position.ToFloat2();
            ID[node] = k++;
            if (node is InputNode)
            {
                template.inCnt++;
            }
            else if (node is OutputNode)
            {
                template.outCnt++;
            }
        }

        template.inputControllers = new();
        template.outputControllers = new();
        /* ----- controllers ----- */
        foreach (var controller in controllers)
        {
            if (controller is MultibitControllerInput mci)
            {
                var cids = new List<int>();
                foreach (var inputNode in mci.Inputs)
                    cids.Add(ID[inputNode]);
                template.inputControllers.Add((controller.Position.ToFloat2(), cids));//(pos, controlled inputs)
            }
            else if (controller is MultibitControllerOutput mco)
            {
                var cids = new List<int>();
                foreach (var outputNode in mco.Outputs)
                    cids.Add(ID[outputNode]);
                template.outputControllers.Add((controller.Position.ToFloat2(), cids));
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

        //AppSaveData.AddTemplate(template);

        return template;
    }
    public static GateTemplate SaveAllAsTemplate(string newName, RenderProperties renderProperties) // TODO: ### jeszcze color i size !!!
    {
        var template = GetTemplateFromAll();
        template.defaultName = newName;
        template.renderProperties = renderProperties;
        AppSaveData.AddTemplate(template);
        return template;
    }

    public static GateTemplate SaveAllAsProject(string projectName)
    {
        var template = GetTemplateFromAll();
        template.defaultName = projectName;
        template.renderProperties = new RenderProperties();
        return template;
    }

    public static GateTemplate SaveNodesAsTemplateALPHA(List<Node> nodes, string templateName)
    {
        throw new Exception("do not try this at home");
        /*
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
        */
    }
}

public static class NodeSearch
{
    private static int CurrentSearchId = 0;
    public static void RunSearchAndCalculateAllNodes(IEnumerable<Node> inputs, IEnumerable<Node> controllers)
    {
        Queue<Node> queue = new();
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

        //na koncu obliczyc controllery, jesli to ma wgl sens 
        //np. jesli jestesmy w wartwie ukrytej to nie ma sensu liczyc controllerow
        if(controllers != null)
        {
            foreach (var controller in controllers)
            {
                controller.Calculate();
            }
        }

        //zeby nastepnym razem inputy sie tez zerowaly
        CurrentSearchId += 1;
    }
}