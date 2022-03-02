using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MBIIC : ItemController
{
    protected override void Start()
    {
        base.Start();

        contextMenuItems.Add(new ContextMenuItem("Switch signed/unsigned", sampleButton,
            x => (x as MultibitControllerInput).Signed = !(x as MultibitControllerInput).Signed));

        contextMenuItems.Add(new ContextMenuItem("Change value", sampleButton,
            x => PlayerController.Instance.OnTypeValue(x as MultibitControllerInput)));
    }
}
