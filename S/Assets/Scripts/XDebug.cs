using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XDebug : MonoBehaviour
{ 
    // Update is called once per frame
    void Start()
    {
        AppSaveData.Load();
        Debug.Log(Application.dataPath);
    }
    InputNode in0;
    InputNode in1;
    Node and0;
    Node nand0;
    Node not0;
    OutputNode out0;
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            var t = (1, 23);
            t.Item1 = 2;//beka no to pair jest niepotrzebne
            t.SaveClass(Application.dataPath + "/niggaaaa.bin");
        }
        if(Input.GetKeyDown(KeyCode.E)) 
        {
            in0 = (InputNode) NodeManager.CreateNode(AppSaveData.InputTemplate);
            in1 = (InputNode) NodeManager.CreateNode(AppSaveData.InputTemplate);
            nand0 = NodeManager.CreateNode(AppSaveData.GetTemplate(5));
            //and0 = NodeManager.CreateNode(AppSaveData.AndTemplate);
            //not0 = NodeManager.CreateNode(AppSaveData.NotTemplate);
            out0 = (OutputNode) NodeManager.CreateNode(AppSaveData.OutputTemplate);

            //NodeManager.Connect(in0, 0, and0, 0);
            //NodeManager.Connect(in1, 0, and0, 1);
            //NodeManager.Connect(and0, 0, not0, 0);
            //NodeManager.Connect(not0, 0, out0, 0);

            NodeManager.Connect(in0, 0, nand0, 0);
            NodeManager.Connect(in1, 0, nand0, 1);
            NodeManager.Connect(nand0, 0, out0, 0);

            Debug.Log("NANDx is set up");
            Recalc();
        }
        if(Input.GetKeyDown(KeyCode.Q))
        {
            in0.FlipValue();
            Recalc();
        }
        if(Input.GetKeyDown(KeyCode.W))
        {
            in1.FlipValue();
            Recalc();
        }
        if(Input.GetKeyDown(KeyCode.F))
        {
            var list = Helper.LoadClass<List<GateTemplate>>(Application.dataPath + "/asdasdasdasd.bin");
            Debug.Log(list.Count);
        }
        //if(Input.GetKeyDown(KeyCode.P))
        //{
        //    NodeManager.SaveAllAsTemplate("NAND");
        //}
        if(Input.GetMouseButtonDown(0))
        {
            //Debug.Log(Input.mousePosition);
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //Debug.Log(mousePos);
            var hit = Physics2D.Raycast(mousePos, Vector2.zero);
            if(hit.collider != null)
                Debug.Log(hit.collider.gameObject);
        }
    }

    void Recalc()
    {
        NodeManager.CalculateAll();
        Debug.Log($"Input: {in0.GetValue()}, {in1.GetValue()}\nOutput: {out0.GetValue()}");
    }

}
