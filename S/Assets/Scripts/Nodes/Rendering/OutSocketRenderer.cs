using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutSocketRenderer : BaseRenderer
{
    public static OutSocketRenderer Make(Node forWho, int outIdx)
    {
        var go = Object.Instantiate(Resources.Load("Sprites/Socket_Out")) as GameObject;
        var rend = go.GetComponent<OutSocketRenderer>();
        var coll = go.GetComponent<OutSocketCollision>();
        coll.sourceNode = forWho;
        coll.outIdx = outIdx;
        return rend;
    }
}
