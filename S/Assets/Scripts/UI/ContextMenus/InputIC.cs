using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputIC : ItemController
{
    protected override void Start()
    {
        base.Start();
        
        if((nodeObj.node as InputNode).Controlled) 
            contextMenuItems.RemoveAt(0);

        contextMenuItems.Add(new ContextMenuItem("Flip bit", sampleButton,
            x => NodeManager.Flip(nodeObj as InputCollision)));
        contextMenuItems.Add(new ContextMenuItem("Change desc.", sampleButton,
            x => PlayerController.Instance.OnChangeDescriptionClick(nodeObj.node)));
    }
}
