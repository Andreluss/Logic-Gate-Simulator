using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputIC : ItemController
{
    protected override void Start()
    {
        base.Start();

        var innode = (nodeOrEdgeObj as InputCollision).InputNode;
        if(innode.Controlled || PlayerController.Instance.Mode == PlayerController.GameMode.Edit) 
            contextMenuItems.RemoveAt(0);

        contextMenuItems.Add(new ContextMenuItem("Flip bit", sampleButton,
            () => NodeManager.Flip(nodeOrEdgeObj as InputCollision)));
        contextMenuItems.Add(new ContextMenuItem("Change desc.", sampleButton,
            () => PlayerController.Instance.ShowChangeDescriptionMenu(innode)));
    }
}
