using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XDebug : MonoBehaviour
{ 
    // Update is called once per frame
    void Start()
    {
        Debug.Log(Application.dataPath);
        RenderProperties renderProperties = new();
        renderProperties.SaveClass<RenderProperties>(Application.dataPath + "/myass.bin");
        AppSaveData.GateTemplates = new GateTemplate[] { new GateTemplate(), new GateTemplate() };
        if(AppSaveData.GateTemplates != null)
            AppSaveData.GateTemplates.SaveClass(Application.dataPath + "/GateTemplates.bin");
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            RenderProperties renderProperties = Helpers.LoadClass<RenderProperties>(Application.dataPath + "/myass.bin");
            Debug.Log(renderProperties);
        }
        if(Input.GetKeyDown(KeyCode.G))
        {
            GateTemplate[] gateTemplates = Helpers.LoadClass<GateTemplate[]>(Application.dataPath + "/GateTemplates.bin");
            Debug.Log(gateTemplates);
        }
    }


}
