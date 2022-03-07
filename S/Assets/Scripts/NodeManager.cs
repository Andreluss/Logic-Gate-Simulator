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

    public static bool UnsavedChangesInBlock { get; set; }
    public static bool UnsavedChangesInProject { get; set; }

    public static bool UnsavedChanges
    {
        get
        {
            if (PlayerController.Instance.Mode == PlayerController.GameMode.Normal)
                return UnsavedChangesInProject;
            else if(PlayerController.Instance.Mode == PlayerController.GameMode.Edit)
                return UnsavedChangesInBlock;
            else 
                return false;
        }
        set
        {
            if (PlayerController.Instance.Mode == PlayerController.GameMode.Normal)
                UnsavedChangesInProject = value;
            else if (PlayerController.Instance.Mode == PlayerController.GameMode.Edit)
                UnsavedChangesInBlock = value;
        }
    }

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
        if (where != null)
            node.Position = (Vector2)where;

        RegisterNode(node);

        if (AppSaveData.Settings.PinInOutToScreenEdges)
            node.GetRenderer()?.HandlePinPosition();

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

    internal static void SplitEdge(EdgeCollision edge, Vector2 pos)
    {
        //copy na wszelki wypadek
        var (A, outidx, B, inidx) = (edge.from, edge.outIdx, edge.to, edge.inIdx);
        Disconnect(A, outidx, B, inidx);

        var split = CreateNode(AppSaveData.SplitTemplate, pos);
        Connect(A, outidx, split, 0);
        Connect(split, 0, B, inidx);
    }

    internal static void DissolveSplit(Split split)
    {
        Node A = split.ins[0].st;
        var outidx = split.ins[0].nd;

        var outs_copy = new List<ValueTuple<Node, int>>(split.outs[0]);
        foreach (var (B, inidx) in outs_copy)
        {
            Debug.Assert(B != null);
            Connect(A, outidx, B, inidx);
        }

        DeleteNode(split);
    }
    internal static void UpdateGateColor(int id, Color color)
    {
        var templ = AppSaveData.GetTemplate(id);
        Debug.Assert(templ.renderProperties != null);
        templ.renderProperties.Color = color;
        AppSaveData.UpdateGate(id, templ);

        //or just Reload Project, nwm w sumie
        foreach (var node in nodes)
        {
            if (node is Gate && node.GetTemplateID() == id)
            {
                (node.GetRenderer() as GateRenderer).Color = color;
            }
        }
        PlayerController.Instance.ReloadHUD(); //chyba serio lepiej zreloadowac projekt
    }

    public static void DeleteNode(Node node, bool recalc = true)
    {
        if (node is not MultibitController)
            nodes.Remove(node);
        else
            controllers.Remove(node);

        if (node is InputNode)
        {
            inputNodes.Remove(node as InputNode);
        }
        else if (node is OutputNode)
        {
            outputNodes.Remove(node as OutputNode);
        }
        else if (node is MultibitControllerInput mcinput)
        {
            foreach (var inputNode in mcinput.Inputs)
            {
                //nodes.Remove(inputNode);
                //inputNodes.Remove(inputNode);
                DeleteNode(inputNode);
            }
        }
        else if (node is MultibitControllerOutput mco)
        {
            foreach (var outnode in mco.Outputs)
            {
                //nodes.Remove(outnode);
                //outputNodes.Remove(outnode);
                DeleteNode(outnode);
            }
        }

        node.Destroy();

        Debug.Log($"Node {node} deleted");//msdlkjl

        if (recalc) CalculateAll();
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
        foreach (var node in nodes)
        {
            if (node.totalInputEdgesCount == 0)
                readyNodes.Add(node);
        }
        NodeSearch.RunSearchAndCalculateAllNodes(readyNodes, controllers);
        NodeSearch.CurrentSearchId += 1;

        UnsavedChanges = true;// !!!!!
    }

    private static bool descriptionsEnabled = true;

    public static void ToggleDestriptions()
    {
        descriptionsEnabled = !descriptionsEnabled;
        foreach (var node in nodes)
        {
            if (node is InputNode)
                (node as InputNode).GetRenderer().ShowDescription(descriptionsEnabled);
            else if (node is OutputNode)
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
        var nodesCopy = new HashSet<Node>(nodes);
        foreach (var node in nodesCopy)
            NodeManager.DeleteNode(node, false);
        nodes.Clear();

        var controllersCopy = new HashSet<Node>(controllers);
        foreach (var controller in controllersCopy)
            DeleteNode(controller);
        controllers.Clear();

        inputNodes.Clear();
        outputNodes.Clear();//na wszelki wypadek ale to i tak jest niby juz puste
    }

    internal static void PinAll()
    {
        foreach (var node in inputNodes)
            if(!node.Controlled)
                node.GetRenderer().HandlePinPosition();
        foreach (var node in outputNodes)
            if (!node.Controlled)
                node.GetRenderer().HandlePinPosition();

        foreach(var c in controllers)
            c.GetRenderer().HandlePinPosition();

        CalculateAll();
    }

    private static GateTemplate GetTemplateFromAll(bool saveInputVals = false)
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

        template.descriptions = new();

        Dictionary<Node, int> ID = new();
        int k = 0;

        // BARDZO WA¯NY MOMENT
        var sorted_nodes = nodes.OrderByDescending(node => node.Position.y);


        foreach (var node in sorted_nodes)//[TODO] check perf
        {
            template.TemplateIDsForEachNode[k] = node.GetTemplateID();
            template.PositionsForEachNode[k] = node.Position.ToFloat2();
            ID[node] = k++;
            if (node is InputNode)
            {
                template.inCnt++;
                if (node.Description != null)
                {
                    template.descriptions.Add((ID[node], node.Description));
                }
            }
            else if (node is OutputNode)
            {
                template.outCnt++;
                if (node.Description != null)
                {
                    template.descriptions.Add((ID[node], node.Description));
                }
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

        if (saveInputVals)
        {
            template.inputValues = new();
            foreach (var inputNode in inputNodes)
            {
                template.inputValues.Add((ID[inputNode], inputNode.GetValue()));
            }
        }

        //AppSaveData.AddTemplate(template);

        return template;
    }
    public static GateTemplate SaveAllAsNewTemplate(string newName, RenderProperties renderProperties) // TODO: ### jeszcze color i size !!!
    {
        var template = GetBlockSaveFromAll(newName, renderProperties);
        AppSaveData.AddTemplate(template);

        return template;
    }
    public static int SaveAllAsNewProject(string name, bool andClose = false)
    {
        int id = AppSaveData.ProjectCnt;

        var newSave = GetProjectSaveFromAll(name);

        //var newSave = GetTemplateFromAll(true);
        //newSave.defaultName = name;
        //newSave.renderProperties = new RenderProperties();

        AppSaveData.AddProject(newSave);

        UnsavedChanges = false;

        return id;
    }

    public static GateTemplate GetProjectSaveFromAll(string projectName)
    {
        var template = GetTemplateFromAll(true);
        template.defaultName = projectName;
        template.renderProperties = new RenderProperties();

        return template;
    }

    public static GateTemplate GetBlockSaveFromAll(string gateName, RenderProperties rendprops)
    {
        var t = GetTemplateFromAll();
        t.defaultName = gateName;
        t.renderProperties = rendprops;

        return t;
    }

    public static GateTemplate SaveChangesToProject(int id)
    {
        Debug.Assert(id < AppSaveData.ProjectCnt);

        var oldSave = AppSaveData.GetProject(id);
        var newSave = GetTemplateFromAll(true);

        newSave.defaultName = oldSave.defaultName;
        newSave.renderProperties = oldSave.renderProperties;
        newSave.gatesAvailableInThisProject = PlayerController.Instance.GatesInCurrentProject;//no ok

        AppSaveData.UpdateProject(id, newSave);

        UnsavedChanges = false;

        return newSave;
    }

    internal static GateTemplate SaveChangesToTemplate(int currentlyEditedBlockID)
    {
        Debug.Assert(currentlyEditedBlockID < AppSaveData.TemplateCnt);

        var oldSave = AppSaveData.GetTemplate(currentlyEditedBlockID);
        var newSave = GetTemplateFromAll(true);

        newSave.defaultName = oldSave.defaultName;
        newSave.renderProperties = oldSave.renderProperties;
        newSave.templateId = currentlyEditedBlockID;//haha jest bug

        AppSaveData.UpdateGate(currentlyEditedBlockID, newSave);

        UnsavedChanges = false;

        return newSave;
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
    public static int CurrentSearchId = 0;
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
        //CurrentSearchId += 1;
    }
}