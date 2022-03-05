using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectProjectMenuController : MonoBehaviour
{
    [SerializeField]
    private GameObject ProjectsContentPanel;
    [SerializeField]
    private Button ProjectButtonPrefab;

    private void Start()
    {
        Debug.Assert(PlayerController.Instance.Mode == PlayerController.GameMode.Menu);
        for (int id = 0; id < AppSaveData.ProjectCnt; id++)
        {
            var project = AppSaveData.GetProject(id);
            var button = Instantiate(ProjectButtonPrefab, ProjectsContentPanel.transform);
            button.GetComponentInChildren<TextMeshProUGUI>().text = project.defaultName;
            button.GetComponent<ProjectButtonCollision>().ProjectID = id;


            //var tex = Helper.LoadPNG(Application.dataPath + $"/thumb/thumb{id}.jpg");
            //if (tex != null)
            //{
            //    var img = button.transform.GetChild(0).GetComponent<Image>();
            //    var spr = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            //    img.overrideSprite = spr;
            //    img.type = Image.Type.Filled;
            //    img.preserveAspect = true;
            //    img.color = Color.white;
            //}

            //var img = button.transform.GetChild(0).GetComponent<Image>(); [TODO] ewentualnie


            var id_copy = id;
            button.onClick.AddListener(delegate { LoadProject(id_copy); });
        }
    }

    public void LoadNewProject()
    {
        PlayerController.Instance.LoadProject(-1);
        Close();
    }

    public void LoadProject(int id)
    {
        PlayerController.Instance.LoadProject(id);
        Close();
    }

    public void Close()
    {
        PlayerController.Instance.Mode = PlayerController.GameMode.Normal;
        Destroy(gameObject);
    }
}
