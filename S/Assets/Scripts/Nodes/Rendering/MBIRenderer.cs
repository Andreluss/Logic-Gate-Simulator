using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public class MBIRenderer : NodeRenderer //to jest taki node ale bez inputow i outputow
{                                       //moze jak sie uda zrefaktoryzowac kod to to jakos ogarne
    private TextMeshPro text;
    public static MBIRenderer Make(MultibitControllerInput forWho)
    {
        //var rend = GetComponent< new MBIRenderer();
        //assign text 
        throw new NotImplementedException();
        //return rend;
    }

    internal void HandleValue(int value)
    {
        text.text = value.ToString();
    }
}
