using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HideTemplateMenuController : MonoBehaviour
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

    public void HideTemplate()
    {
        PlayerController.Instance.HideTemplateForever(WhatTemplate);
        Debug.Log($"Template {AppSaveData.GetTemplate(WhatTemplate).defaultName} succesfully hidden.");
        Close();
    }

    public void Close()
    {
        Destroy(gameObject.transform.parent.gameObject);
        PlayerController.Instance.LoadHUD();
    }
}
