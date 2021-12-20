using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager : Singleton<NodeManager>
{
    HashSet<Node> nodes;

    public void CreateNode(GateTemplate template, Vector2? where = null)
    {
        Node node = template.BuildNodeFromTemplate();
        node.Position = (Vector2)where;
        node.Hidden = false;

        nodes.Add(node);
    }
    
}
