using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeCollision : CollisionData
{
    public Node from;
    public int outIdx;
    public Node to;
    public int inIdx;
    private EdgeRenderer edgeRenderer;
    public EdgeRenderer EdgeRenderer { get => edgeRenderer; set => edgeRenderer = value; }
    public override BaseRenderer Renderer { get => EdgeRenderer; }

    public void Initialize(Node from, int outIdx, Node to, int inIdx, EdgeRenderer edgeRenderer)
    {
        this.from = from;
        this.outIdx = outIdx;
        this.to = to;
        this.inIdx = inIdx;
        this.EdgeRenderer = edgeRenderer;
    }
}
