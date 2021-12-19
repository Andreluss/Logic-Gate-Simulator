using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Node
{
    protected int inCnt, outCnt;
    public Tuple<Node, int>[] outs, ins;
    public bool[] inVals, outVals; // -> xbfs
    protected Node(int inputCount, int outputCount, string name)
    {
        inCnt = inputCount;
        ins = new Tuple<Node, int>[inCnt];
        inVals = new bool[inCnt];

        outCnt = outputCount;
        outs = new Tuple<Node, int>[outCnt];
        outVals = new bool[outCnt];

        Name = name;
    }
    public virtual void Build() 
    { 
        //Don't do anything - by default
    }
    public virtual void Calculate() 
    { 
        //Same here
    }
    public bool Hidden { get; set; }
    public string Name { get; set; }

    protected Vector3 position;
    public GameObject[] edgeRenderer;
    public GameObject nodeRenderer = null;
}
