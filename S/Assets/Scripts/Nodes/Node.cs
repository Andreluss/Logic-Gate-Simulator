using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Node
{
    public int inCnt, outCnt;
    public Pair<Node, int>[] ins;
    public List<ValueTuple<Node, int>>[] outs; //wsm takie listy sasiedztwa
    
    // NodeNet Search Data, troche syf ale niech na razie bedzie
    public int lastSearchId = -1, processedInCurrentSearch = 0;
    public int totalInputEdgesCount = 0;
    public bool[] inVals, outVals; 

    public Node(int inputCount, int outputCount, string name, bool hidden)
    {
        inCnt = inputCount;
        ins = new Pair<Node, int>[inCnt];
        inVals = new bool[inCnt];

        outCnt = outputCount;
        outs = new List<ValueTuple<Node, int>>[outCnt];//tablica list wyjsc
        for (int i = 0; i < outCnt; i++)
        {
            outs[i] = new();
        }
        outVals = new bool[outCnt];

        Name = name;
        Hidden = hidden;//name conflict?
    }
    public virtual int GetTemplateID()
    {
        return -1;
    }
    private bool IsFree(int inIdx)
    {
        //TODO: checks for inIdx
        return ins[inIdx].st == null;
    }
    protected virtual void HandleNewInputConnection(int inIdx, Node from, int fromOutIdx)
    {
        totalInputEdgesCount += 1;
        ins[inIdx] = new Pair<Node, int>(from, fromOutIdx);
    }
    public virtual void ConnectTo(int outIdx, Node to, int inIdx)
    {
        if(!Helper.InRange(outIdx, 0, outCnt) || !Helper.InRange(inIdx, 0, to.inCnt))
        {
            throw new Exception("In/out socket idx out of range");
        }
        if (!to.IsFree(inIdx))
        {
            throw new Exception("Dest. node input is already full");
        }
        //TODO: GUI - create edge and make a new edgeRenderer if not Hidden
        //( ## or do that in NodeManager???)
        outs[outIdx].Add(new ValueTuple<Node, int>(to, inIdx));
        if(!hidden)
        {
            if (this.GetRenderer() == null || to.GetRenderer() == null)
                throw new Exception("connected nodes don't have renderers");
            GetRenderer().MakeEdgeTo(outIdx, to, inIdx);
            //var newEdge = EdgeRenderer.Make(this, outIdx, to, inIdx);
            //outEdgeRenderers[outIdx].Add(newEdge);
            //to.inEdgeRenderers[inIdx] = newEdge;
        }
        to.HandleNewInputConnection(inIdx, this, outIdx);
    }

    public virtual void Calculate()
    {
        //Same here
    }

    protected virtual void CreateRenderer()
    {
        throw new Exception("your ass");
    }

    protected virtual void DestroyRenderer()
    {
        throw new Exception("calling this method makes no sense");
    }

    public virtual NodeRenderer GetRenderer() => null;

    public bool Hidden
    {
        get => hidden;
        set
        {
            if (hidden == value) return;
            hidden = value;
            if (hidden == true)
            {
                this.DestroyRenderer();//[TODO] destroy all
                //[CHYBA] to nie jest potrzebne wgl

                //outEdgeRenderers = null;
                //foreach (var outRendList in outEdgeRenderers)
                //{
                //    foreach (var outRend in outRendList)
                //    {
                //        UnityEngine.Object.Destroy(outRend.gameObject);
                //    }
                //}
                //inEdgeRenderers = null;
                //foreach (var inRend in inEdgeRenderers)
                //{
                //    UnityEngine.Object.Destroy(inRend.gameObject);
                //}
            }
            else
            {
                this.CreateRenderer();
                GetRenderer().UpdatePosition(position);
                //outEdgeRenderers = new List<EdgeRenderer>[outCnt];
                //for (int i = 0; i < outCnt; i++)
                //    outEdgeRenderers[i] = new List<EdgeRenderer>();
                //inEdgeRenderers = new EdgeRenderer[inCnt];
            }
        }
    }
    public Vector2 Position
    {
        get => position;
        set
        {
            position = value;
            var rend = GetRenderer();
            if (rend)
                rend.UpdatePosition(position);
        }
    }
    public string Name { get => name; set => name = value; }

    //public List<EdgeRenderer>[] outEdgeRenderers;
    //public EdgeRenderer[] inEdgeRenderers;
    //public GameObject nodeRenderer = null;

    private bool hidden = true;
    private Vector2 position;
    private string name;
}
