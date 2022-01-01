using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InSocketRenderer : BaseRenderer
{
    public static InSocketRenderer Make(Node forWho, int inIdx)
    {
        var go = Object.Instantiate(Resources.Load("Sprites/Socket_In")) as GameObject;
        var rend = go.GetComponent<InSocketRenderer>();
        var coll = go.GetComponent<InSocketCollision>();
        coll.targetNode = forWho;
        coll.inIdx = inIdx;
        return rend;
    }

}
