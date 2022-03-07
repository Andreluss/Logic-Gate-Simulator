using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIIC : ItemController, IPointerEnterHandler, IPointerExitHandler
{
    protected override void Start()
    {
        base.Start();

        if (rClickableObj is NodeTemplateCollision)
        {
            var id = (rClickableObj as NodeTemplateCollision).TemplateID;
            //contextMenuItems.Add(new ContextMenuItem("Remove from this list", sampleButton, () => PlayerController.Instance.ShowHideTemplateMenu(id)));

            if (id > 10)
            {
                //jesli nie s¹ to podstawowe bramki
                
                if(AppSaveData.Settings.ShowAllGates)
                {
                    contextMenuItems.Add(new ContextMenuItem("<color=\"red\"><b>Remove completely</b></color>", sampleButton,
                    () => PlayerController.Instance.ShowHideTemplateMenu(id)));
                }
                else
                {
                    contextMenuItems.Add(new ContextMenuItem("Remove from the list", sampleButton,
                    () => PlayerController.Instance.RemoveTemplateFromProjectList(id)));
                }
            }
        }
        //else if (rClickableObj as ProjectButtonCollision)
        //{
        //    var id = (rClickableObj as ProjectButtonCollision).ProjectID;

        //    contextMenuItems.Add(new ContextMenuItem("Delete project", sampleButton,
        //    () => PlayerController.Instance.ShowDeleteProjectMenu(id)));
        //}
    }

    private bool isMouseOver = false;

    public bool IsMouseOver { get => isMouseOver; set => isMouseOver = value; }

    private void LateUpdate()
    {
        if (IsMouseOver)
        {
            CheckRMBClick();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        IsMouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        IsMouseOver = false;
    }
}
