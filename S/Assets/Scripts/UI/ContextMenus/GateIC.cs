using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateIC : ItemController
{
    protected override void Start()
    {
        base.Start();

        var gateRend = nodeOrEdgeObj.Renderer as GateRenderer;

        contextMenuItems.Add(new ContextMenuItem("Change color", sampleButton,
            () =>  PlayerController.Instance.ShowChangeColorMenu(gateRend)));

        if(gateRend.node is GateComplex)
        {
            if(PlayerController.Instance.Mode != PlayerController.GameMode.Edit)
            {
                var id = (nodeOrEdgeObj as GateCollision).node.GetTemplateID();
                contextMenuItems.Add(new ContextMenuItem("Edit", sampleButton,
                () => PlayerController.Instance.SwitchToEditMode(id)));
            }
        }
    }
}
