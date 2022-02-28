using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutSocketRenderer : BaseRenderer
{
    public Material materialON, materialOFF;
    private SpriteRenderer spriteRenderer;
    public static OutSocketRenderer Make(Node forWho, int outIdx)
    {
        var go = Object.Instantiate(Resources.Load("Sprites/Socket_Out")) as GameObject;
        var rend = go.GetComponent<OutSocketRenderer>();
        rend.spriteRenderer = rend.GetComponent<SpriteRenderer>();
        Debug.Assert(rend.spriteRenderer != null);
        var coll = go.GetComponent<OutSocketCollision>();
        coll.sourceNode = forWho;
        coll.outIdx = outIdx;
        return rend;
    }
    public void HandleState(bool ON)
    {
        if (ON)
            spriteRenderer.material = materialON;
        else
            spriteRenderer.material = materialOFF;
    }
}
