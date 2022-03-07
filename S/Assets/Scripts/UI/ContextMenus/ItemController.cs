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
    protected CollisionData rClickableObj;

    void Awake()
    {
        Debug.Log("lkjshadlkfjh");
        camera = Camera.main;
        if (sampleButton == null)
            sampleButton = Resources.Load<Button>("Sprites/UI/ContextButton");
    }

    protected virtual void Start()
    {
        rClickableObj = gameObject.GetComponent<CollisionData>();
        contextMenuItems = new List<ContextMenuItem>();
        if (rClickableObj is NodeCollision)
            contextMenuItems.Add(new ContextMenuItem("Delete", sampleButton, () => NodeManager.DeleteNode((rClickableObj as NodeCollision).node)));
        else if (rClickableObj is EdgeCollision)
        {
            var edge = (EdgeCollision)rClickableObj;
            contextMenuItems.Add(new("Delete edge", sampleButton, () => NodeManager.Disconnect(edge.from, edge.outIdx, edge.to, edge.inIdx)));
            contextMenuItems.Add(new("Split edge", sampleButton, () => NodeManager.SplitEdge(edge, GetMousePosition())));
        }

        //[TODO] copy node (moze nawet selected nodes)
    }

    public void CheckRMBClick()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if(contextMenuItems.Count > 0)
            {
                //Vector3 pos = transform.position;
                Vector3 pos = GetMousePosition();
                ContextMenu.Instance.CreateContextMenu(contextMenuItems, new Vector2(pos.x, pos.y));
            }
        }
    }

    void OnMouseOver()
    {
        CheckRMBClick();
    }

    private Vector2 GetMousePosition()
    {
        return camera.ScreenToWorldPoint(Input.mousePosition);
    }
}
