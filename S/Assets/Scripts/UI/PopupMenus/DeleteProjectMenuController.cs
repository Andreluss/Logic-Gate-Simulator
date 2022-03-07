using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeleteProjectMenuController : MonoBehaviour
{
    private int whatProject;
    [SerializeField]
    private TextMeshProUGUI title;
    private GateTemplate t;

    public int WhatProject
    {
        get => whatProject; set
        {
            whatProject = value;
            t = AppSaveData.GetProject(WhatProject);
            title.text = $"Do you really want to delete\nthe <color=#{ColorUtility.ToHtmlStringRGB(t.renderProperties.Color)}>{t.defaultName}</color> project?";
        }
    }

    public void DeleteProject()
    {
        PlayerController.Instance.HideTemplateForever(WhatProject);
        Debug.Log($"Project {t.defaultName} succesfully deleted.");
        Close();
    }

    public void Close()
    {
        Destroy(gameObject.transform.parent.gameObject);
    }
}
