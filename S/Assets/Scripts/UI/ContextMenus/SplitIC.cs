using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitIC : ItemController
{
    protected override void Start()
    {
        base.Start();

        var split = (nodeOrEdgeObj.Renderer as SplitRenderer).node as Split;
        if(split.ins[0].st != null) //zeby bylo co dissolvowac
            contextMenuItems.Add(new("Dissolve", sampleButton, () => NodeManager.DissolveSplit(split)));
    }
}
