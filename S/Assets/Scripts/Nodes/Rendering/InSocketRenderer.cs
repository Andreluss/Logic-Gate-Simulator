using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InSocketRenderer : BaseRenderer
{
    public Material materialON, materialOFF;
    private SpriteRenderer spriteRenderer;
    public static InSocketRenderer Make(Node forWho, int inIdx)
    {
        var go = Object.Instantiate(Resources.Load("Sprites/Socket_In")) as GameObject;
        var rend = go.GetComponent<InSocketRenderer>();
        rend.spriteRenderer = rend.GetComponent<SpriteRenderer>();
        Debug.Assert(rend.spriteRenderer != null);
        var coll = go.GetComponent<InSocketCollision>();
        coll.targetNode = forWho;
        coll.inIdx = inIdx;
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
