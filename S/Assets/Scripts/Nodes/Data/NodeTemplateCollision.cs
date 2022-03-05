using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NodeTemplateCollision : CollisionData
{
    public int TemplateID;
    public string NodeName
    {
        get => nodeName;
        set
        {
            nodeName = value;
            transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = nodeName;
        }
    }
    private Color color;
    private string nodeName;
    public override BaseRenderer Renderer { get => GetComponent<BaseRenderer>(); }
    public Color Color
    {
        get => color; set
        {
            color = value;
            //GetComponent<Image>().color = color.Dim(0.9f);
            transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = color;/* = $"<color=#{ColorUtility.ToHtmlStringRGB(value)}>{NodeName}</color>";*/
        }
    }
}
