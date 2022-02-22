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
                if(outEdgeRend != null)
                {
                    outEdgeRend.Start = outSocketRends[i].transform.position;
                }
            }
        }
    }
    public void ConnectTo(int outIdx, Node to, int inIdx)
    {
        var newEdge = EdgeRenderer.Make(node, outIdx, to, inIdx);
        outEdgeRenderers[outIdx].Add(newEdge);
        to.GetRenderer().inEdgeRenderers[inIdx] = newEdge;
    }
    //public EdgeRenderer MakeEdgeToPoint(int outIdx, Vector2 point)
    //{
    //    var newEdge = EdgeRenderer.Make(node, outIdx, point);
    //    return newEdge;
    //}
    public void RemoveEdgeWith(int outIdx, Node with, int inIdx)
    {
        //[TODO] zoptymalizowac to troche bo az zal
        //for (int i = 0; i < outEdgeRenderers[outIdx].Count; i++)
        //{
        //    Debug.Assert(outEdgeRenderers[outIdx][i] != null);
        //    if(outEdgeRenderers[outIdx][i].to == with &&
        //       outEdgeRenderers[outIdx][i].inIdx == inIdx)
        //    {
        //        outEdgeRenderers[outIdx][i].Destroy();
        //        outEdgeRenderers[outIdx].RemoveAt(i);
        //        break;
        //    }
        //}
        int rendidx = 0;
        foreach (EdgeRenderer outEdgeRend in outEdgeRenderers[outIdx])
        {
            Debug.Assert(outEdgeRend != null);
            if (outEdgeRend.to == with && outEdgeRend.inIdx == inIdx)
            {
                outEdgeRend.Destroy();
                outEdgeRenderers[outIdx].RemoveAt(rendidx);
                break;
            }
            rendidx++;
        }
        with.GetRenderer().inEdgeRenderers[inIdx] = null;
    }
}
