using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveAsNewProjectMenuController : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField inputField;
    [SerializeField]
    private TextMeshProUGUI warning;
    [SerializeField]
    private Button createButton;

    //private RenderProperties rendprops = new();

    public void TestName()
    {
        if (AppSaveData.ProjectExists(inputField.text))
        {
            warning.gameObject.SetActive(true);
            createButton.interactable = false;//??
        }
        else if (inputField.text.Length == 0)
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
        PlayerController.Instance.SaveAsNewProject(inputField.text);
        Close();
    }

    public void Close()
    {
        Destroy(gameObject.transform.parent.gameObject);
        PlayerController.Instance.LoadHUD();
    }
}
