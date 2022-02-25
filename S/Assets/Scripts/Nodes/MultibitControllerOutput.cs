using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultibitControllerOutput : MultibitController //wlasciwie taki meta node
{
    public MultibitControllerOutput(int bitCount, bool hidden) : base(bitCount, hidden)
    {
        //...
        //dopasuj rozmiar calego kloca
        Outputs = new List<OutputNode>();
        for (int i = 0; i < bitCount; i++)
        {
            //create new OutputNode
            Vector2 pos = new Vector2(0, -bitCount + i); // <== [TODO] wyliczyc to
            Outputs[i] = NodeManager.CreateNode(AppSaveData.OutputTemplate, pos) as OutputNode;
            //Outputs[i].Controller = this;
        }
    }
    public MultibitControllerOutput(List<OutputNode> forOutputs, bool hidden) : base(forOutputs.Count, hidden)
    {
        throw new NotImplementedException();
    }

    public List<OutputNode> Outputs { get; }
}
