using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutputIC : ItemController
{
    protected override void Start()
    {
        base.Start();
        
        if((nodeObj.node as OutputNode).Controlled) 
            contextMenuItems.RemoveAt(0);
        
        contextMenuItems.Add(new ContextMenuItem("Change description", sampleButton,
            x => PlayerController.Instance.OnChangeDescriptionClick(nodeObj.node)));
    }
}
