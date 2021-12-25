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
        if(Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("my ass");
            List<GateTemplate> templates = new();
            templates.Add(new GateTemplate());
            templates.Add(new GateTemplate());
            templates.Add(new GateTemplate());
            templates.Add(new GateTemplate());
            templates.SaveClass(Application.dataPath + "/asdasdasdasd.bin");
            Debug.Log("my ass 2");
        }
        if(Input.GetKeyDown(KeyCode.Q))
        {
            var list = Helpers.LoadClass<List<GateTemplate>>(Application.dataPath + "/asdasdasdasd.bin");
            Debug.Log(list.Count);
        }
    }


}
