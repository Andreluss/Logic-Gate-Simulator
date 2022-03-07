using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsController : MonoBehaviour
{
    [SerializeField]
    private GameObject OptionList;
    [SerializeField]
    private GameObject Blocker;

    bool isOpen = false;
    public void Toggle()
    {
        if (!isOpen)
            Open();
        else
            Close();
    }

    public void Open()
    {
        isOpen = true;
        OptionList.SetActive(true);
        Blocker.SetActive(true);
    }

    //private void Update()
    //{
    //    if(isOpen)
    //    {
    //        if(Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
    //        {
    //            Close();
    //        }
    //    }
    //}

    public void Close()
    {
        isOpen = false;
        OptionList.SetActive(false);
        Blocker.SetActive(false);
    }
}
