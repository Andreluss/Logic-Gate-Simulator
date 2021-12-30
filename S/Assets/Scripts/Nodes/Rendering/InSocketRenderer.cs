using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InSocketRenderer : BaseRenderer
{
    public static InSocketRenderer Make(Node forWho, int inIdx)
    {
        var go = Object.Instantiate(Resources.Load("Sprites/Socket_In")) as GameObject;
        var rend = go.AddComponent<InSocketRenderer>();
        var coll = go.AddComponent<InSocketCollision>();
        coll.targetNode = forWho;
        coll.inIdx = inIdx;
        return rend;
    }

}
