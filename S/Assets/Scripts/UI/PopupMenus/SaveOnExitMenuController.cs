using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveOnExitMenuController : MonoBehaviour
{
    public void SaveChanges()
    {
        PlayerController.Instance.OnSaveClick(true);
        Close();
    }

    public void DontSaveChanges()
    {
        PlayerController.Instance.Mode = PlayerController.GameMode.Menu;
        Close();
    }

    public void Close()
    {
        Destroy(gameObject.transform.parent.gameObject);
        Debug.Log("anulowano");
    }
}
