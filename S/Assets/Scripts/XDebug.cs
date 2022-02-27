using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XDebug : MonoBehaviour
{ 
    // Update is called once per frame
    void Start()
    {
        NodeManager.CreateNode(AppSaveData.InputTemplate, new Vector2(-4, -2));
        NodeManager.CreateNode(AppSaveData.InputTemplate, new Vector2(-4, 2));
        NodeManager.CreateNode(AppSaveData.OutputTemplate, new Vector2(4, 0));
        //NodeManager.CreateNode(AppSaveData.GetTemplate(11), new Vector2(0, 0));

        NodeManager.CreateNode(AppSaveData.AndTemplate);
        NodeManager.CreateNode(AppSaveData.NotTemplate);

        Debug.Log("NANDx is set up");
    }

    void Update()
    {
    }

}
