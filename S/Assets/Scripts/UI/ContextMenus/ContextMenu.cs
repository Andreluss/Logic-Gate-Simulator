using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class ContextMenuItem
{
    // this class - just a box to some data

    public string text;             // text to display on button
    public Button button;           // sample button prefab
    public Action action;    // delegate to method that needs to be executed when button is clicked

    public ContextMenuItem(string text, Button button, Action action)
    {
        this.text = text;
        this.button = button;
        this.action = action;
    }
}

public class ContextMenu : Singleton<ContextMenu>
{
    public Image contentPanelPrefab;              // content panel prefab
    public Canvas canvas;                   // link to main canvas, where will be Context Menu
    private Image panel;

    private void Awake()
    {
        contentPanelPrefab = Resources.Load<Image>("Sprites/UI/ContextPanel");
    }
    public void DestroyContextMenu()
    {
        if (panel != null) 
            Destroy(panel.gameObject);
    }
    public void CreateContextMenu(List<ContextMenuItem> items, Vector2 position)
    {
        DestroyContextMenu();

        panel = Instantiate(contentPanelPrefab/*, Vector3.zero, Quaternion.identity*/);
        panel.transform.SetParent(canvas.transform, false); 
        panel.transform.position = position;
        //panel.transform.SetAsLastSibling();
        //panel.rectTransform.anchoredPosition = position;

        foreach (var item in items)
        {
            //ContextMenuItem tempReference = item;
            Button button = Instantiate(item.button) as Button;
            var buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            //var buttonText = button.GetComponentInChildren<Text>();
            buttonText.text = item.text;
            button.onClick.AddListener(delegate { item.action(); Destroy(panel.gameObject); });
            button.transform.SetParent(panel.transform, false);
        }

    }
}