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
    public bool SaveAndClose;

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

    public void CreateProject()
    {
        PlayerController.Instance.SaveAsNewProject(inputField.text, SaveAndClose);
        Close();
        PlayerController.Instance.screenShotManager.SaveScreenShot(AppSaveData.ProjectCnt-1);//[bug?] troche syf ale to nie taka wazna rrzecz
    }

    public void Close()
    {
        Destroy(gameObject.transform.parent.gameObject);
        if (SaveAndClose)
            PlayerController.Instance.Mode = PlayerController.GameMode.Menu;
        else
            PlayerController.Instance.LoadHUD();
    }
}
