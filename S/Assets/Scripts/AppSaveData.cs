﻿// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
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
    //2  -> Split
    //3  -> And
    //4  -> Not
    //5+ -> <reszta>
    public static GateTemplate GetTemplate(int id)
    {
        return GateTemplates[id];
    }
    public static GateTemplate InputTemplate { get => GateTemplates[0]; }
    public static GateTemplate OutputTemplate { get => GateTemplates[1]; }
    public static GateTemplate SplitTemplate { get => GateTemplates[2]; }
    public static GateTemplate AndTemplate { get => GateTemplates[3]; }
    public static GateTemplate NotTemplate { get => GateTemplates[4]; }
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
    private static string gatePath = Application.dataPath + "/GateTemplates.bin";

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
            for (int i = 0; i < 5; i++)
            {
                gateTemplates.Add(new GateTemplate());
                gateTemplates[i].NodeType = (NodeType)i;
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
