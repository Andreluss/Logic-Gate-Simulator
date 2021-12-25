// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
// Pamietac, że kolejnosc inputu i outputu
// jest wazna bo sie laczy z internalnymi
// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
using UnityEngine;

public static class AppSaveData
{
    private static GateTemplate[] gateTemplates;
    //0  -> Input
    //1  -> Output
    //2  -> Split
    //3  -> And
    //4  -> Not
    //5+ -> <reszta>
    //workaround:
    public static GateTemplate[] GateTemplates {
        get
        {
            if(gateTemplates == null)
            {
                gateTemplates = Helpers.LoadClass<GateTemplate[]>(Application.dataPath + "/GateTemplates.bin");
            }
            return gateTemplates;
        }
        set => gateTemplates = value; 
    }

    public static int InputTemplateID = new InputNode().GetTemplateID();
    public static int OutputTemplateID = new OutputNode().GetTemplateID();
    //TODO: load this at the start
}
