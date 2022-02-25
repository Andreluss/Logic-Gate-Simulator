// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
// Pamietac, że kolejnosc inputu i outputu
// jest wazna bo sie laczy z internalnymi
// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
using UnityEngine;
using System.Collections.Generic;
using System.IO;
public static class AppSaveData
{
    //0  -> Input
    //1  -> Output
    //2  -> MultibitInput (2)
    //3  -> MultibitInput (4)
    //4  -> MultibitInput (8)
    //5  -> MultibitOutput (2) 
    //6  -> MultibitOutput (4)
    //7  -> MultibitOutput (8)
    //8  -> Split
    //9  -> And
    //10 -> Not
    //7+ -> <reszta>
    public static GateTemplate GetTemplate(int id)
    {
        return GateTemplates[id];
    }
    public static GateTemplate InputTemplate { get => GateTemplates[0]; }
    public static GateTemplate OutputTemplate { get => GateTemplates[1]; }
    public static GateTemplate SplitTemplate { get => GateTemplates[8]; }
    public static GateTemplate AndTemplate { get => GateTemplates[9]; }
    public static GateTemplate NotTemplate { get => GateTemplates[10]; }
    public static int TemplateCnt { get => GateTemplates.Count; }

    public static void AddTemplate(GateTemplate newTemplate)
    {
        newTemplate.templateId = GateTemplates.Count;
        GateTemplates.Add(newTemplate);
        GateTemplates.SaveClass(Application.dataPath + "/GateTemplates.bin");
    }
    public static bool TemplateExists(string name)
    {
        return GateTemplates.Find(x => x.defaultName == name) != null;
    }

    public static int InputTemplateID = new InputNode(true).GetTemplateID();
    public static int OutputTemplateID = new OutputNode(true).GetTemplateID();

    private static List<GateTemplate> GateTemplates;
    private static readonly string gatePath = Application.dataPath + "/GateTemplates.bin";

    public struct Settings
    {
        public static bool SnapObjects = false;
        public static float SnapDistance = 0.5f;
    }


    public static void Load()
    {
        if (!File.Exists(gatePath)) // jesli nie istnieje to odtwarzamy 5 podstawowych bramek
        {
            List<GateTemplate> gateTemplates = new();
            string[] names = {"In", "Out", "Split", "NOT", "AND"};
            for (int i = 0; i < 5; i++)
            {
                gateTemplates.Add(new GateTemplate());
                gateTemplates[i].NodeType = (NodeType)i;
                gateTemplates[i].defaultName = names[i];
            }
            gateTemplates.SaveClass(gatePath);
        }
        GateTemplates = Helper.LoadClass<List<GateTemplate>>(gatePath);
        if (GateTemplates == null)
        {
            Debug.LogError("my ass");
        }
        Debug.Log($"App save data loaded succesfully from \"{Application.dataPath}\"");

    }
}
