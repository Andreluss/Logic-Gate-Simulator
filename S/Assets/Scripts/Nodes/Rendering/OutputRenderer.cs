using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutputRenderer : BaseRenderer
{
    public InSocketRenderer inSocketRenderer;
    public static OutputRenderer Make(OutputNode forWho)
    {
        var outputRootGO = Instantiate(Resources.Load<GameObject>
                                        ("Sprites/OutputRoot"));
        var outputGO = outputRootGO.transform.GetChild(0);
        var outputRend = outputGO.GetComponent<OutputRenderer>();
        Vector2 dim = outputGO.GetComponent<SpriteRenderer>().size;

        var socket = InSocketRenderer.Make(forWho, 0);
        socket.transform.SetParent(outputRootGO.transform, false);
        socket.transform.localPosition = new Vector3(-dim.x / 2,
                                                     0,
                                                     socket.transform.localPosition.z);
        outputRend.inSocketRenderer = socket;

        var coll = outputGO.GetComponent<OutputCollision>();
        coll.outputNode = forWho;

        return outputRend;
    }
}
