using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateCollision : NodeCollision
{
    private Gate gate;

    public Gate Gate { get => (Gate)node; }
}
