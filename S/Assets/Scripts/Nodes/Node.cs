using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Node
{
    protected int inCnt, outCnt;
    public Tuple<Node, int>[] ins;
    public List<Tuple<Node, int>>[] outs; //wsm takie listy sasiedztwa
    public bool[] inVals, outVals; // -> xbfs
    public GateTemplate template;
    public Node(int inputCount, int outputCount, string name, bool hidden = false)
    {
        inCnt = inputCount;
        ins = new Tuple<Node, int>[inCnt];
        inVals = new bool[inCnt];

        outCnt = outputCount;
        outs = new List<Tuple<Node, int>>[outCnt];//tablica list wyjsc
        outVals = new bool[outCnt];

        Name = name;
        Hidden = hidden;//name conflict?
    }
    //protected void Init(int inputCount, int outputCount, string name)
    //{

    //}
    //public virtual void Build() 
    //{
    //    throw new Exception("??");
    //}
    public bool IsFree(int inIdx)
    {
        //TODO: checks for inIdx
        return ins[inIdx].Item1 == null;
    }
    public virtual void HandleNewInputConnection(int inIdx) { }
    public virtual void ConnectTo(int outIdx, Node to, int inIdx)
    {
        if (!to.IsFree(inIdx))
        {
            throw new Exception("Dest. node input is already full");
        }
        //TODO: GUI - create edge and make a new edgeRenderer if not Hidden
        outs[outIdx].Add(new Tuple<Node, int>(to, inIdx));
        to.HandleNewInputConnection(inIdx);
    }
    public virtual void Calculate()
    {
        //Same here
    }
    public bool Hidden { get => hidden; set => hidden = value; }
    public Vector2 Position { get => position; set => position = value; }
    public string Name { get => name; set => name = value; }

    public GameObject[] edgeRenderer;
    public GameObject nodeRenderer = null;

    private bool hidden;
    private Vector2 position;
    private string name;
}
