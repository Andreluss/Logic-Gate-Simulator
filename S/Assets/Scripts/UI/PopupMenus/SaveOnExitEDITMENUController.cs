using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveOnExitEDITMENUController : MonoBehaviour
{
    public void SaveChanges()
    {
        PlayerController.Instance.OnEMSaveClick(true);
        Close();
    }

    public void DontSaveChanges()
    {
        PlayerController.Instance.Mode = PlayerController.GameMode.Normal;
        NodeManager.UnsavedChangesInBlock = false;
        Close();
    }

    public void Close()
    {
        Destroy(gameObject.transform.parent.gameObject);
    }
}
