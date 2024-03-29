﻿// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
// Pamietac, że kolejnosc inputu i outputu
// jest wazna bo sie laczy z internalnymi
// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

public static class AppSaveData
{
    //0  -> Input
    //1  -> MultibitInput (2)
    //2  -> MultibitInput (4)
    //3  -> MultibitInput (8)
    //4  -> Output
    //5  -> MultibitOutput (2) 
    //6  -> MultibitOutput (4)
    //7  -> MultibitOutput (8)
    //8  -> Split
    //9 -> Not
    //10  -> And
    //11+ -> <reszta>
    public static GateTemplate GetTemplate(int id)
    {
        return GateTemplates[id];
    }
    public static GateTemplate InputTemplate { get => GateTemplates[0]; }
    public static GateTemplate OutputTemplate { get => GateTemplates[4]; }
    public static GateTemplate SplitTemplate { get => GateTemplates[8]; }
    public static GateTemplate NotTemplate { get => GateTemplates[9]; }
    public static GateTemplate AndTemplate { get => GateTemplates[10]; }
    public static int TemplateCnt { get => GateTemplates.Count; }
    public static int ProjectCnt { get => Projects.Count; }

    public static void AddTemplate(GateTemplate newTemplate)
    {
        newTemplate.templateId = GateTemplates.Count;
        GateTemplates.Add(newTemplate);
        GateTemplates.SaveClass(gatePath);
    }
    public static bool TemplateExists(string name)
    {
        return GateTemplates.Find(x => x.defaultName == name) != null;
    }

    public static GateTemplate GetProject(int id)
    {
        return Projects[id];
    }
    public static void AddProject(GateTemplate project)
    {
        project.templateId = ProjectCnt;
        Projects.Add(project);
        Projects.SaveClass(projectPath);
    }
    public static void UpdateProject(int id, GateTemplate newProjectData)
    {
        Debug.Assert(Helper.InRange(id, 0, ProjectCnt));
        Projects[id] = newProjectData;
        Projects.SaveClass(projectPath);
    }
    public static bool ProjectExists(string name)
    {
        return Projects.Find(x => x.defaultName == name) != null;
    }


    public static int InputTemplateID = new InputNode(true).GetTemplateID();
    public static int OutputTemplateID = new OutputNode(true).GetTemplateID();

    private static List<GateTemplate> GateTemplates;
    private static readonly string gatePath = Application.dataPath + "/data/GateTemplates.bin";
    private static readonly string projectPath = Application.dataPath + "/data/Projects.bin";

    internal static void UpdateGate(int id, GateTemplate newTemplateData)
    {
        GateTemplates[id] = newTemplateData;
        GateTemplates.SaveClass(gatePath);//chyba git ??
    }

    public static List<GateTemplate> Projects;

    public struct Settings
    {
        public static bool SnapObjects = true;
        public static float SnapDistance = 0.5f;
        public static bool ShowAllGates = false;
        public static bool PinInOutToScreenEdges = true;
    }


    public static void Load()
    {
        /* Gate stuff */
        if (!File.Exists(gatePath)) // jesli nie istnieje to odtwarzamy 5 podstawowych bramek
        {
            System.IO.Directory.CreateDirectory(Application.dataPath + "/data");
            List <GateTemplate> gateTemplates = new();
            string[] names = {"In", "In2bit", "In4bit", "In8bit",
                              "Out", "Out2bit", "Out4bit", "Out8bit",
                              "Split", "NOT", "AND"};
            int[] bitc = { 0, 2, 4, 8, 0, 2, 4, 8, 0, 0, 0 };
            NodeType[] types = {NodeType.Input, NodeType.MultibitInput, NodeType.MultibitInput, NodeType.MultibitInput,
                                NodeType.Output, NodeType.MultibitOutput, NodeType.MultibitOutput, NodeType.MultibitOutput,
                                NodeType.Split, NodeType.NOT, NodeType.AND};
            RenderProperties[] rendprops = { new RenderProperties(Color.white),
                                             new RenderProperties(Color.white),
                                             new RenderProperties(Color.white),
                                             new RenderProperties(Color.white),
                                             new RenderProperties(Color.white),
                                             new RenderProperties(Color.white),
                                             new RenderProperties(Color.white),
                                             new RenderProperties(Color.white),
                                             new RenderProperties(Color.white),
                                             new RenderProperties(Color.white),
                                             new RenderProperties(Color.white) };
            for (int i = 0; i < names.Length; i++)
            {
                var t = new GateTemplate();
                t.NodeType = types[i];
                t.defaultName = names[i];
                t.bitCount = bitc[i];
                t.renderProperties = rendprops[i];
                t.templateId = i;
                gateTemplates.Add(t);
            }
            gateTemplates.SaveClass(gatePath);
        }
        GateTemplates = Helper.LoadClass<List<GateTemplate>>(gatePath);
        Debug.Assert(GateTemplates != null);

        /* Projects files */
        if (!File.Exists(projectPath))
            Projects = new();
        else 
            Projects = Helper.LoadClass<List<GateTemplate>>(projectPath);

    }

    internal static void HideTemplate(int id)
    {
        var t = GetTemplate(id);
        t.DELETED = true;
        UpdateGate(id, t);
    }
}
