using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OutputRenderer : NodeRenderer
{
    private TextMeshPro text;
    private TextMeshPro description;
    RectTransform rt;
    private void Awake()
    {
        outline = transform.parent.GetChild(3).gameObject;
        rt = transform.parent.GetComponent<RectTransform>();
    }
    public override void EnableOutline()
    {
        outline.SetActive(true);
    }
    public override void DisableOutline()
    {
        outline.SetActive(false);
    }
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
        outputRend.description = outputRootGO.transform.GetChild(2).GetComponent<TextMeshPro>();

        var coll = outputGO.GetComponent<OutputCollision>();
        coll.node = forWho;

        return outputRend;
    }

    public void UpdateDescription()
    {
        description.text = Node.Description;
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
    public override void HandlePinPosition()
    {
        Vector3 stageBorders = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        float x = stageBorders.x;
        float nodeWidth = rt.rect.width;
        var newPos = node.Position;

        newPos.x = x - nodeWidth / 2 * 1.05f;

        node.Position = newPos;
    }
}
