using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public class MBIRenderer : NodeRenderer //to jest taki node ale bez inputow i outputow
{                                       //moze jak sie uda zrefaktoryzowac kod to to jakos ogarne
    private void Awake()
    {
        outline = transform.parent.GetChild(2).gameObject;
    }
    public override void EnableOutline()
    {
        outline.SetActive(true);
    }
    public override void DisableOutline()
    {
        outline.SetActive(false);
    }
    private TextMeshPro text;
    public static MBIRenderer Make(MultibitControllerInput forWho)
    {
        var rootGO = Instantiate(Resources.Load("Sprites/InputControllerRoot") as GameObject);
        var GO = rootGO.transform.GetChild(0).gameObject;
        
        /* calculate and set the scale */ 
        Vector2 dim = new Vector2(1.5f, 1.5f + forWho.BitCount * 1.25f);
        rootGO.GetComponent<RectTransform>().sizeDelta = dim;
        GO.transform.localScale = dim;

        var rend = GO.GetComponent<MBIRenderer>();
        rend.text = rootGO.GetComponentInChildren<TextMeshPro>();
        rend.Node = forWho;//??

        rend.outline.transform.localScale = (Vector3)dim + (Vector3)Vector2.one * 0.05f;

        var coll = GO.GetComponent<MBICollision>();
        coll.node = forWho;

        return rend;
    }

    internal void HandleValue(int value)
    {
        string str = value.ToString();
        if ((Node as MultibitControllerInput).Signed && value > 0)
            str = "+" + str;
        text.text = str;
    }
    public override void UpdatePosition(Vector2 position, bool sdfsdf = false)
    {
        gameObject.transform.parent.position = new Vector3(position.x,
                                                           position.y,
                                                           gameObject.transform.parent.position.z);
        var inputs = (Node as MultibitControllerInput).Inputs;
        if (inputs != null)
        {
            foreach (var inputNode in inputs)
            {
                //if (inputNode.GetRenderer() != null)
                inputNode.GetRenderer()?.UpdatePosition(position, true);
            }
        }
    }
}
