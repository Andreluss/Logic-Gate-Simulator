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


    bool changes;

    private void Start()
    {
        backup = node.Description;
        inputField.text = backup;//[DESIGN]
        changes = NodeManager.UnsavedChanges;
    }

    public void UpdateDescription()
    {
        node.Description = inputField.text;
        NodeManager.UnsavedChanges = true;
    }

    public void Cancel()
    {
        node.Description = backup;
        NodeManager.UnsavedChanges = changes;
        Close();
    }

    public void Close()
    {
        Destroy(gameObject.transform.parent.gameObject);
    }
}
