using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MBOIC : ItemController
{
    protected override void Start()
    {
        base.Start();

        contextMenuItems.Add(new ContextMenuItem("Switch Signed/Unsigned mode", sampleButton,
            x => (x as MultibitControllerOutput).Signed = !(x as MultibitControllerOutput).Signed));
    }
}