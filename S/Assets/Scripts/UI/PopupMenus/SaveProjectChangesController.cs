using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveProjectChangesController : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField inputField;
    [SerializeField]
    private TextMeshProUGUI warning;
    [SerializeField]
    private Button createButton;

    private RenderProperties rendprops = new();


    public void SaveChanges()
    {
        PlayerController.Instance.OnSaveClick();
        Close();
    }

    public void Close()
    {
        Destroy(gameObject.transform.parent.gameObject);
    }
}
