using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MBOIC : ItemController
{
    protected override void Start()
    {
        base.Start();

        if (PlayerController.Instance.Mode == PlayerController.GameMode.Edit)
            contextMenuItems.RemoveAt(0);

        var ctrl = (rClickableObj as MBOCollision).node;
        contextMenuItems.Add(new ContextMenuItem("Switch Signed/Unsigned", sampleButton,
            () => (ctrl as MultibitControllerOutput).Signed = !(ctrl as MultibitControllerOutput).Signed));
    }
}