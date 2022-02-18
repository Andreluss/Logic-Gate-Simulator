using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OutputRenderer : NodeRenderer
{
    private TextMeshPro text;
    public static OutputRenderer Make(OutputNode forWho)
    {
        var outputRootGO = Instantiate(Resources.Load<GameObject>
                                        ("Sprites/OutputRoot"));
        var outputGO = outputRootGO.transform.GetChild(0);
        var outputRend = outputGO.GetComponent<OutputRenderer>();
        outputRend.Node = forWho;
        Vector2 dim = outputGO.GetComponent<SpriteRenderer>().size;

        var socket = InSocketRenderer.Make(forWho, 0);
        socket.transform.SetParent(outputRootGO.transform, false);
        socket.transform.localPosition = new Vector3(-dim.x / 2,
                                                     0,
                                                     socket.transform.localPosition.z);
        outputRend.inSocketRends = new InSocketRenderer[] { socket };

        outputRend.text = outputRootGO.GetComponentInChildren<TextMeshPro>();

        var coll = outputGO.GetComponent<OutputCollision>();
        coll.outputNode = forWho;

        return outputRend;
    }

    internal void HandleValue(bool value)
    {
        text.text = value ? "1" : "0";
        if (value) // 1
        {

        }
        else // 0
        {

        }
    }
}
