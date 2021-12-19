using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Node
{
    protected int inCnt, outCnt;
    public Tuple<Node, int>[] outs, ins;
    public bool[] inVals, outVals; // -> xbfs
    protected Node(int inputCount, int outputCount)
    {
        inCnt = inputCount;
        ins = new Tuple<Node, int>[inCnt];
        inVals = new bool[inCnt];

        outCnt = outputCount;
        outs = new Tuple<Node, int>[outCnt];
        outVals = new bool[outCnt];
    }
    public virtual void Build() 
    { 
        //Don't do anything - by default
    }
    public void Calculate() 
    { 
        //Same here
    }
    public bool Hidden { get; set; }
    public string Name { get; set; }
    protected Vector3 position;
    public GameObject[] edgeRenderer;
    public GameObject nodeRenderer = null;
}

public class InputNode : Node
{
    public InputNode(string name = "Input"): base(0, 1)
    {
        Name = name;
    }
}

public class OutputNode : Node
{
    public OutputNode() : base(1, 0)
    {

    }
}
public class GateNOT : Node
{
    public GateNOT(): base(1, 1)
    {
        Name = "NOT";
    }

    public override void Calculate()
    {
        outVals[0] = !inVals[0];
    }
}

public class GateAND : Node
{
    public GateAND() : base(2, 1)
    {
        Name = "AND";
    }

    public override void Calculate()
    {
        outVals[0] = inVals[0] && inVals[1];
    }
}
