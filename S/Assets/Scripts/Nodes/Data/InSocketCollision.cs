using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InSocketCollision : CollisionData
{
    public Node targetNode;
    public int inIdx;
    public override BaseRenderer Renderer { get => targetNode?.GetRenderer(); }
}
