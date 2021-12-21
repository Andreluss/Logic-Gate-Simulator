// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
// Pamietac, że kolejnosc inputu i outputu
// jest wazna bo sie laczy z internalnymi
// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
using UnityEngine;

public static class AppSaveData
{
    private static GateTemplate[] gateTemplates;
    //workaround:
    public static GateTemplate[] GateTemplates {
        get
        {
            if(gateTemplates == null) 
                gateTemplates = Helpers.LoadClass<GateTemplate[]>(Application.dataPath + "/GateTemplates.bin");
            return gateTemplates;
        }
        set => gateTemplates = value; 
    }
    //TODO: load this at the start
}
