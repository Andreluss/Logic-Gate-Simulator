// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
// Pamietac, że kolejnosc inputu i outputu
// jest wazna bo sie laczy z internalnymi
// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
using UnityEngine;
using System.Collections.Generic;

public static class AppSaveData
{
    //0  -> Input
    //1  -> Output
    //2  -> Split
    //3  -> And
    //4  -> Not
    //5+ -> <reszta>
    public static GateTemplate GetTemplate(int id)
    {
        return GateTemplates[id];
    }
    public static void AddTemplate(GateTemplate newTemplate)
    {
        newTemplate.templateId = GateTemplates.Count;
        GateTemplates.Add(newTemplate);
        GateTemplates.SaveClass(Application.dataPath + "/GateTemplates.bin");
    }

    public static int InputTemplateID = new InputNode().GetTemplateID();
    public static int OutputTemplateID = new OutputNode().GetTemplateID();

    private static List<GateTemplate> GateTemplates;
    static AppSaveData()
    {
        GateTemplates = Helpers.LoadClass<List<GateTemplate>>(Application.dataPath + "/GateTemplates.bin");
        if(GateTemplates == null)
        {
            Debug.LogError("my ass");
        }
    }
    //TODO: load this at the start
}
