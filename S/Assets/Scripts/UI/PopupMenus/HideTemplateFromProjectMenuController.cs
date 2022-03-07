using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HideTemplateFromProjectMenuController : MonoBehaviour
{
    private int whatTemplate;
    [SerializeField]
    private TextMeshProUGUI title;

    public int WhatTemplate
    {
        get => whatTemplate; set
        {
            whatTemplate = value;
            var t = AppSaveData.GetTemplate(WhatTemplate);
            title.text = $"Do you really want to remove\nthe <color=#{ColorUtility.ToHtmlStringRGB(t.renderProperties.Color)}>{t.defaultName}</color> gate from the list?";
        }
    }

    public void RemoveTemplateFromProjectList()
    {
        PlayerController.Instance.RemoveTemplateFromProjectList(WhatTemplate);
        Debug.Log($"Template {AppSaveData.GetTemplate(WhatTemplate).defaultName} succesfully removed from project's list.");
        Close();
    }

    public void Close()
    {
        Destroy(gameObject.transform.parent.gameObject);
        PlayerController.Instance.ReloadHUD();
    }
}
