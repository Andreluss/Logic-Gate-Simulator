using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeRenderer : BaseRenderer
{
    public OutSocketRenderer[] outSocketRends;
    public InSocketRenderer[] inSocketRends;
    public void UpdatePosition(Vector2 position)
    {
        //[DANGER] we assume the prefabs have a 'root structure'!
        gameObject.transform.parent.position = position;
    }
}
