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
    [SerializeField]
    private GameObject colorPanel;
    [SerializeField]
    private GameObject colorButtonPrefab;

    private RenderProperties rendprops = new();

    private void Awake()
    {
        int[] hex = { 0xff94a6, 0xffa529, 0xcc9927, 0xf7f47c, 0xbffb00, 0x1aff2f, 0x25ffa8, 0x5cffe8, 0x8bc5ff, 0x5480e4, 0x92a7ff, 0xd86ce4, 0xe553a0, 0xffffff, 0xff3636, 0xf66c03, 0x99724b, 0xfff034, 0x87ff67, 0x3dc300, 0x00bfaf, 0x19e9ff, 0x10a4ee, 0x007dc0, 0x886ce4, 0xb677c6, 0xff39d4, 0xd0d0d0, 0xe2675a, 0xffa374, 0xd3ad71, 0xedffae, 0xd2e498, 0xbad074, 0x9bc48d, 0xd4fde1, 0xcdf1f8, 0xb9c1e3, 0xcdbbe4, 0xae98e5, 0xe5dce1, 0xa9a9a9, 0xc6928b, 0xb78256, 0x99836a, 0xbfba69, 0xa6be00, 0x7db04d, 0x88c2ba, 0x9bb3c4, 0x85a5c2, 0x8393cc, 0xa595b5, 0xbf9fbe, 0xbc7196, 0x7b7b7b, 0xaf3333, 0xa95131, 0x724f41, 0xdbc300, 0x85961f, 0x539f31, 0x0a9c8e, 0x236384, 0x1a2f96, 0x2f52a2, 0x624bad, 0xa34bad, 0xcc2e6e, 0x3c3c3c };
        foreach (var h in hex)
        {
            var b = Instantiate<GameObject>(colorButtonPrefab, colorPanel.transform);
            b.GetComponent<Image>().color = Helper.ColorFromHex(h);
            var button = b.GetComponent<Button>();
            button.onClick.AddListener(delegate { SetColorFromButton(button); });
        }
    }


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
