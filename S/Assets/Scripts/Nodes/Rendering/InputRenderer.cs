using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputRenderer : NodeRenderer
{
    private TextMeshPro text;
    public static InputRenderer Make(InputNode forWho)
    {
        var inputRootGO = Instantiate(Resources.Load<GameObject>
                                     ("Sprites/InputRoot"));
        var inputGO = inputRootGO.transform.GetChild(0).gameObject;
        var inputRend = inputGO.GetComponent<InputRenderer>();
        Vector2 dim = inputGO.GetComponent<SpriteRenderer>().size;

        var socket = OutSocketRenderer.Make(forWho, 0);
        socket.transform.SetParent(inputRootGO.transform, false);
        float zIndex = socket.transform.localPosition.z;
        socket.transform.localPosition = new Vector3(+dim.x / 2,
                                                     0,
                                                     zIndex);
        inputRend.outSocketRends = new OutSocketRenderer[] { socket };

        inputRend.text = inputRootGO.GetComponentInChildren<TextMeshPro>();

        var coll = inputGO.GetComponent<InputCollision>();
        coll.inputNode = forWho;

        return inputRend;
    }

    internal void HandleValue(bool value)
    {
        text.text = value ? "1" : "0";
    }
}
