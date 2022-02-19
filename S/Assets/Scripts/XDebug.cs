using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XDebug : MonoBehaviour
{ 
    // Update is called once per frame
    void Start()
    {
        AppSaveData.Load();
        in0 = (InputNode)NodeManager.CreateNode(AppSaveData.InputTemplate, new Vector2(-4, -2));
        in1 = (InputNode)NodeManager.CreateNode(AppSaveData.InputTemplate, new Vector2(-4, 2));
        nand0 = NodeManager.CreateNode(AppSaveData.GetTemplate(5), new Vector2(0, 0));
        //and0 = NodeManager.CreateNode(AppSaveData.AndTemplate);
        //not0 = NodeManager.CreateNode(AppSaveData.NotTemplate);
        out0 = (OutputNode)NodeManager.CreateNode(AppSaveData.OutputTemplate, new Vector2(4, 0));

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
    InputNode in0;
    InputNode in1;
    Node and0;
    Node nand0;
    Node not0;
    OutputNode out0;
    CollisionData selectedObject;
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E)) 
        {
            if(selectedObject is InputCollision)
            {
                (selectedObject as InputCollision).InputNode.FlipValue();
            }
            else if(selectedObject is GateCollision)
            {
                (selectedObject as GateCollision).Gate.Position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            if (selectedObject is GateCollision)
            {
                (selectedObject as GateCollision).Gate.Destroy();
            }
            else if (selectedObject is InputCollision)
            {
                (selectedObject as InputCollision).InputNode.Destroy();
            }
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
        //if(Input.GetKeyDown(KeyCode.F))
        //{
        //    var list = Helper.LoadClass<List<GateTemplate>>(Application.dataPath + "/asdasdasdasd.bin");
        //    Debug.Log(list.Count);
        //}
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
            {
                //Debug.Log(hit.collider.gameObject);
                selectedObject = hit.collider.gameObject.GetComponentInChildren<CollisionData>();
                Debug.Assert(selectedObject != null);
                Debug.Log($"Selected obj is {selectedObject.name}");
            }
        }
    }

    void Recalc()
    {
        NodeManager.CalculateAll();
        Debug.Log($"Input: {in0.GetValue()}, {in1.GetValue()}\nOutput: {out0.GetValue()}");
    }

}
