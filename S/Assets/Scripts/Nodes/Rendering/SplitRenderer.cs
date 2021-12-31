using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitRenderer : BaseRenderer
{
    public static SplitRenderer Make(Split forWho)
    {
        var go = Resources.Load<GameObject>("Sprites/Split");
        var rend = go.GetComponent<SplitRenderer>();
        var coll = go.GetComponent<SplitCollision>();
        coll.split = forWho;
        return rend;
    }
}
