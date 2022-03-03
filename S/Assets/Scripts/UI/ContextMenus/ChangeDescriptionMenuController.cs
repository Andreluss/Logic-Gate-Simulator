using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChangeDescriptionMenuController : MonoBehaviour
{

    [SerializeField]
    private TMP_InputField inputField;

    public Node node;
    private string backup;

    private void Start()
    {
        backup = node.Description;
        inputField.text = backup;//[DESIGN]
    }

    public void UpdateDescription()
    {
        node.Description = inputField.text;
    }

    public void Cancel()
    {
        node.Description = backup;
        Close();
    }

    public void Close()
    {
        Destroy(gameObject.transform.parent.gameObject);
    }
}
