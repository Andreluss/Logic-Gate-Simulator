using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutSocketCollision : CollisionData
{
    public Node sourceNode;
    public int outIdx;
    public override BaseRenderer Renderer { get => sourceNode?.GetRenderer(); }
}
