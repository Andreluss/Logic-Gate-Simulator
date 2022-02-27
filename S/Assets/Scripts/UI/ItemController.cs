using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemController : MonoBehaviour
{
    public Button sampleButton;                         // sample button prefab
    private List<ContextMenuItem> contextMenuItems;     // list of items in menu

    void Awake()
    {
        // Here we are creating and populating our future Context Menu.
        // I do it in Awake once, but as you can see, 
        // it can be edited at runtime anywhere and anytime.

        contextMenuItems = new List<ContextMenuItem>();
        Action<Image> equip = new(EquipAction);
        Action<Image> use = new Action<Image>(UseAction);
        Action<Image> drop = new Action<Image>(DropAction);

        contextMenuItems.Add(new ContextMenuItem("Equip", sampleButton, equip));
        contextMenuItems.Add(new ContextMenuItem("Use", sampleButton, use));
        contextMenuItems.Add(new ContextMenuItem("Drop", sampleButton, drop));
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
            Debug.DrawRay(pos, Vector3.forward);
            ContextMenu.Instance.CreateContextMenu(contextMenuItems, new Vector2(pos.x, pos.y));
        }

    }

    void EquipAction(Image contextPanel)
    {
        Debug.Log("Equipped");
        Destroy(contextPanel.gameObject);
    }

    void UseAction(Image contextPanel)
    {
        Debug.Log("Used");
        Destroy(contextPanel.gameObject);
    }

    void DropAction(Image contextPanel)
    {
        Debug.Log("Dropped");
        Destroy(contextPanel.gameObject);
    }
}
