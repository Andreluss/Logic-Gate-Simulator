using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemController : MonoBehaviour
{
    public Button sampleButton;                         // sample button prefab
    [SerializeField]
    protected List<ContextMenuItem> contextMenuItems;     // list of items in menu
    protected new Camera camera;
    protected CollisionData nodeOrEdgeObj;

    void Awake()
    {
        Debug.Log("lkjshadlkfjh");
        camera = Camera.main;
        if (sampleButton == null)
            sampleButton = Resources.Load<Button>("Sprites/UI/ContextButton");
    }

    protected virtual void Start()
    {
        nodeOrEdgeObj = gameObject.GetComponent<CollisionData>();
        contextMenuItems = new List<ContextMenuItem>();
        if (nodeOrEdgeObj is NodeCollision)
            contextMenuItems.Add(new ContextMenuItem("Delete", sampleButton, () => NodeManager.DeleteNode((nodeOrEdgeObj as NodeCollision).node)));
        else if (nodeOrEdgeObj is EdgeCollision)
        {
            var edge = (EdgeCollision)nodeOrEdgeObj;
            contextMenuItems.Add(new("Delete", sampleButton, () => NodeManager.Disconnect(edge.from, edge.outIdx, edge.to, edge.inIdx)));
        }
        else
            throw new Exception("wrong object for context menu");//[CHECK] this 

        //[TODO] copy node (moze nawet selected nodes)
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            //Vector3 pos = transform.position;
            Vector3 pos = GetMousePosition();
            ContextMenu.Instance.CreateContextMenu(contextMenuItems, new Vector2(pos.x, pos.y));
        }

    }

    private Vector2 GetMousePosition()
    {
        return camera.ScreenToWorldPoint(Input.mousePosition);
    }
}
