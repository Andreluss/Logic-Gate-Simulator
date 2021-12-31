using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputRenderer : BaseRenderer
{
    public OutSocketRenderer outSocketRend;
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
        inputRend.outSocketRend = socket;

        var coll = inputGO.GetComponent<InputCollision>();
        coll.inputNode = forWho;

        return inputRend;
    }

    internal void HandleValue(bool value)
    {
        throw new NotImplementedException();
    }
}
