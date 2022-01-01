using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeCollision : MonoBehaviour
{
    public Node from; 
    public int outIdx;
    public Node to; 
    public int inIdx;
    public EdgeRenderer edgeRenderer;
    public void Initialize(Node from, int outIdx, Node to, int inIdx, EdgeRenderer edgeRenderer)
    {
        this.from = from;
        this.outIdx = outIdx;
        this.to = to;
        this.inIdx = inIdx;
        this.edgeRenderer = edgeRenderer;
    }
}
