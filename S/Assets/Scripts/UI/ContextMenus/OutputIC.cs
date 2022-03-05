using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutputIC : ItemController
{
    protected override void Start()
    {
        base.Start();

        var outnode = (rClickableObj as OutputCollision).OutputNode;
        if (outnode.Controlled || PlayerController.Instance.Mode == PlayerController.GameMode.Edit) 
            contextMenuItems.RemoveAt(0);

        contextMenuItems.Add(new ContextMenuItem("Change desc.", sampleButton,
            () => PlayerController.Instance.ShowChangeDescriptionMenu(outnode)));
    }
}
