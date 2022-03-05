using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HideTemplateMenuController : MonoBehaviour
{
    public int whatTemplate;
    [SerializeField]
    private TextMeshProUGUI title;

    private void Awake()
    {
        title.text = $"Do you really want to remove <color=#{ColorUtility.ToHtmlStringRGB(AppSaveData.GetTemplate(whatTemplate).renderProperties.Color)}>XOR</color> gate from the list?";
    }

    public void HideTemplate()
    {
        PlayerController.Instance.HideTemplateForever(whatTemplate);
        Debug.Log($"Template {AppSaveData.GetTemplate(whatTemplate).defaultName} succesfully hidden.");
        Close();
    }

    public void Close()
    {
        Destroy(gameObject.transform.parent.gameObject);
        PlayerController.Instance.LoadHUD();
    }
}
