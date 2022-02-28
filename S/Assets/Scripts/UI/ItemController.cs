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
    protected NodeCollision nodeObj;

    void Awake()
    {
        Debug.Log("lkjshadlkfjh");
        camera = Camera.main;
        if (sampleButton == null)
            sampleButton = Resources.Load<Button>("Sprites/UI/ContextButton");
    }

    protected virtual void Start()
    {
        nodeObj = gameObject.GetComponent<NodeCollision>();
        contextMenuItems = new List<ContextMenuItem>();
        contextMenuItems.Add(new ContextMenuItem("Delete", sampleButton, x => NodeManager.DeleteNode(x)));
        //[TODO] copy node (moze nawet selected nodes)
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            //Vector3 pos = transform.position;
            Vector3 pos = GetMousePosition();
            ContextMenu.Instance.CreateContextMenu(nodeObj.node, contextMenuItems, new Vector2(pos.x, pos.y));
        }

    }

    private Vector2 GetMousePosition()
    {
        return camera.ScreenToWorldPoint(Input.mousePosition);
    }
}
