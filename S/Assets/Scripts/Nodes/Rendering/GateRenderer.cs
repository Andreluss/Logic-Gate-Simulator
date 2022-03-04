using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GateRenderer : NodeRenderer
{
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
    public static GateRenderer Make(Gate forWho)
    {
        var gateRootGO = Instantiate(Resources.Load("Sprites/GateRoot")) as GameObject; 
        Debug.Log($"{gateRootGO.name} loaded");
        var gateGO = gateRootGO.transform.GetChild(0).gameObject;
        var gateRend = gateGO.GetComponent<GateRenderer>();

        //root (with TMP component)
        //--gate (scaled acc. to calculated DIMENSIONS (or from render props))
        //--inSocket0
        //--inSocket1
        //--outSocket0
        //...

        var TMP = gateRootGO.GetComponentInChildren<TextMeshPro>();
        TMP.text = forWho.Name;//pierwszy chyba raz sie to przydaje (chociaz [TODO] i tak lepiej to brac z template'a)





        /* ============ rendering part ============ */

        int id = forWho.GetTemplateID();
        var rendprops = AppSaveData.GetTemplate(id).renderProperties;
        gateRend.Color = rendprops.Color;

        int c = Mathf.Max(forWho.inCnt, forWho.outCnt);
        float y = 1.2f + (c - 1) * 0.5f;
        var dim = gateRend.transform.localScale = new Vector2(2, y);


        gateRend.outline.transform.localScale = dim + (Vector3)Vector2.one * 0.05f;





        float width = dim.x;
        float height = dim.y;
        float distIn = height / (forWho.inCnt + 1);//odleglosc miedzy kolejnych socketami (OY)
        float distOut = height / (forWho.outCnt + 1);

        //gateRend.gate = forWho;
        gateRend.Node = forWho;
        gateRend.inSocketRends = new InSocketRenderer[forWho.inCnt];
        for (int i = 0; i < gateRend.inSocketRends.Length; i++)
        {
            var socket = InSocketRenderer.Make(forWho, i);
            //relative position
            socket.transform.SetParent(gateRootGO.transform, false);
            float zIndex = socket.transform.localPosition.z;
            socket.transform.localPosition = new Vector3(-width / 2,
                                                         -height / 2 + distIn * (i + 1),
                                                         zIndex);
            gateRend.inSocketRends[i] = socket;
        }

        gateRend.outSocketRends = new OutSocketRenderer[forWho.outCnt];
        for (int i = 0; i < gateRend.outSocketRends.Length; i++)
        {
            var socket = OutSocketRenderer.Make(forWho, i);
            socket.transform.SetParent(gateRootGO.transform, false);
            float zIndex = socket.transform.localPosition.z;
            socket.transform.localPosition = new Vector3(width / 2,
                                                         -height / 2 + distOut * (i + 1),
                                                         zIndex);
            gateRend.outSocketRends[i] = socket;
        }

        var coll = gateGO.GetComponent<GateCollision>();
        coll.node = forWho;
        return gateRend;
    }

    private Color color;
    public Color Color { get => color; set => color = transform.parent.GetChild(1).GetComponent<TextMeshPro>().color = value; }
}
