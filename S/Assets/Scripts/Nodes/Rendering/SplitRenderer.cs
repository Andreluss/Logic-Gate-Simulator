using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitRenderer : NodeRenderer
{
    public static SplitRenderer Make(Split forWho)
    {
        var rootGO = Instantiate(Resources.Load<GameObject>("Sprites/SplitRoot"));
        var splitGO = rootGO.transform.GetChild(0);
        var rend = splitGO.GetComponent<SplitRenderer>();
        rend.Node = forWho;

        float width = splitGO.GetComponent<SpriteRenderer>().size.x;

        var inSocket = InSocketRenderer.Make(forWho, 0);
        inSocket.transform.SetParent(rootGO.transform, false);
        inSocket.transform.localPosition = new Vector3(-width / 2,
                                                       0,
                                                       inSocket.transform.localPosition.z);
        rend.inSocketRends = new InSocketRenderer[] { inSocket };
        
        var outSocket = OutSocketRenderer.Make(forWho, 0);
        outSocket.transform.SetParent(rootGO.transform, false);
        outSocket.transform.localPosition = new Vector3(+width / 2,
                                                        0,
                                                        outSocket.transform.localPosition.z);
        rend.outSocketRends = new OutSocketRenderer[] { outSocket };

        var coll = splitGO.GetComponent<SplitCollision>();
        coll.node = forWho;

        return rend;
    }
}
