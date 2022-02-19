using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeRenderer : BaseRenderer
{
    private Node node;//!!! musi byc przypisany przy tworzeniu
    int inCnt, outCnt;
    public OutSocketRenderer[] outSocketRends;
    public InSocketRenderer[] inSocketRends;
    public List<EdgeRenderer>[] outEdgeRenderers;
    public EdgeRenderer[] inEdgeRenderers;

    protected Node Node
    {
        get => node;
        set
        {
            node = value;
            if(node != null)
            {
                //[TODO] tutaj jeszcze odtworzyc krawedzie
                inCnt = value.inCnt;
                outCnt = value.outCnt;

                outEdgeRenderers = new List<EdgeRenderer>[outCnt];
                for (int i = 0; i < outCnt; i++)
                    outEdgeRenderers[i] = new List<EdgeRenderer>();
                inEdgeRenderers = new EdgeRenderer[inCnt];
            }
        }
    }

    public void UpdatePosition(Vector2 position)
    {
        //[DANGER] we assume the prefabs have a 'root structure'!
        gameObject.transform.parent.position = position;
        for (int i = 0; i < inCnt; i++)
        {
            if(inEdgeRenderers[i] != null)
            {
                inEdgeRenderers[i].End = inSocketRends[i].transform.position;
            }
        }

        for (int i = 0; i < outCnt; i++)
        {
            foreach (EdgeRenderer outEdgeRend in outEdgeRenderers[i])
            {
                outEdgeRend.Start = outSocketRends[i].transform.position;
            }
        }
    }
    public void MakeEdgeTo(int outIdx, Node to, int inIdx)
    {
        var newEdge = EdgeRenderer.Make(node, outIdx, to, inIdx);
        outEdgeRenderers[outIdx].Add(newEdge);
        to.GetRenderer().inEdgeRenderers[inIdx] = newEdge;
    }
    
    public void RemoveEdgeWith(int outIdx, Node with, int inIdx)
    {
        //[TODO] zoptymalizowac to troche bo az zal
        foreach(EdgeRenderer outEdgeRend in outEdgeRenderers[outIdx])
        {
            if(outEdgeRend.to == with && outEdgeRend.inIdx == inIdx)
            {
                outEdgeRend.Destroy();
            }
        }
        with.GetRenderer().inEdgeRenderers[inIdx] = null;
    }
}
