using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeTemplateCollision : CollisionData
{
    public int TemplateID;
    public string Name;
    public override BaseRenderer Renderer { get => null; }
}
