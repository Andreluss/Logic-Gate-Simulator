using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutputIC : ItemController
{
    protected override void Start()
    {
        base.Start();

        var outnode = (nodeOrEdgeObj as OutputCollision).OutputNode;
        if (outnode.Controlled) 
            contextMenuItems.RemoveAt(0);

        contextMenuItems.Add(new ContextMenuItem("Change description", sampleButton,
            () => PlayerController.Instance.OnChangeDescriptionClick(outnode)));
    }
}
