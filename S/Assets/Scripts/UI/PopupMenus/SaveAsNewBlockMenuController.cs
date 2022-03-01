using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveAsNewBlockMenuController : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField inputField;
    [SerializeField]
    private TextMeshProUGUI warning;
    [SerializeField]
    private Button createButton;

    private RenderProperties rendprops = new();

    public void TestName()
    {
        if(AppSaveData.TemplateExists(inputField.text))
        {
            warning.gameObject.SetActive(true);
            createButton.interactable = false;//??
        }
        else if(inputField.text.Length == 0)
        {
            warning.gameObject.SetActive(false);
            createButton.interactable = false;
        }
        else
        {
            warning.gameObject.SetActive(false);
            createButton.interactable = true;
        }
    }

    public void CreateTemplate()
    {
        Debug.Log("nigga whass cracking?");//NodeManager.SaveAllAsTemplate(inputField.text, rendprops);
        PlayerController.Instance.SaveAllAsNewTemplate(inputField.text, rendprops);
        Close();
    }

    public void SetColorFromButton(Button button)
    {
        rendprops.Color = button.GetComponent<Image>().color;
        createButton.GetComponent<Image>().color = rendprops.Color;
    }


    public void Close()
    {
        Destroy(gameObject.transform.parent.gameObject);
        PlayerController.Instance.LoadHUD();
    }
}
