using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using System;

[RequireComponent(typeof(StateMachine))]
public class PlayerController : Singleton<PlayerController>
{
    private Camera m_Camera;
    private Camera c_Camera;
    private StateMachine StateMachine;

    /* okienka */
    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private GameObject SaveAsNewBlockMenu;
    [SerializeField]
    private GameObject SaveOnExitMenu;
    [SerializeField]
    private GameObject SaveAsNewProjectMenu;

    /* Dane aktualnego projektu */
    public int CurrentProjectID { get => currentProjectID; set => currentProjectID = value; }
    public string CurrentProjectName { get => CurrentProjectID == -1 ? "Untitled" : AppSaveData.GetProject(CurrentProjectID).defaultName;}




    private void Awake()
    {
        m_Camera = Camera.main;
        c_Camera = GameObject.Find("Const Camera").GetComponent<Camera>();

        AppSaveData.Load();
        LoadHUD();
        StateMachine = GetComponent<StateMachine>();
        StateMachine.Initialize(new StateMachine.PlayerState(StateIdle));
        PlayerState = State.Idle;

        //UnityEngine.Assertions.Assert.raiseExceptions = true;
    }




    void Update()
    {
        StateMachine.UpdateStateMachine();
        if (PlayerState == State.Idle)
        {
            //wkurza mnie ten warning
            //Debug.Log("is over UI: " + IsPointerOverUIObject());
        }
        var delta = -Input.mouseScrollDelta.y;
        if (Mathf.Abs(delta) > 0)
        {
            Debug.Log(Input.mouseScrollDelta);
            m_Camera.orthographicSize = Mathf.Clamp(m_Camera.orthographicSize + delta * 0.75f, 0.25f, 100000f);
            c_Camera.orthographicSize = m_Camera.orthographicSize;
        }
    }





    /* Klikniêcia guzików */
    public void OnToggleDescriptions()
    {
        //ahh bo Unity nie widzi menegera
        NodeManager.ToggleDestriptions();
    }
    public void OnToggleSnapping()
    {
        AppSaveData.Settings.SnapObjects = !AppSaveData.Settings.SnapObjects;
    }
    public void OnChangeDescriptionClick(Node node)
    {
        throw new System.NotImplementedException();
    }
    public void OnTypeValue(MultibitController controller)
    {
        throw new System.NotImplementedException();
    }
    public void OnSaveClick() //(Ctrl + S)
    {
        if (CurrentProjectID == -1)
        {
            //save as new project or cancel
            ShowSaveAsNewProjectMenu();
        }
        else
        {
            //just save changes
            SaveChanges();
        }

    }






    /* wyœwietlacze menu itp. */
    public void ShowSaveAsNewBlockMenu()
    {
        Instantiate(SaveAsNewBlockMenu, canvas.transform);
    }
    public void ShowSaveAsNewProjectMenu()
    {
        Instantiate(SaveAsNewProjectMenu, canvas.transform);
    }
    public void ShowSaveOnExitMenu()
    {
        Instantiate(SaveOnExitMenu, canvas.transform);
    }






    /* w³aœciwe operacje ju¿ po wyklikaniu opcji z menu */
    public void SaveAllAsNewTemplate(string name, RenderProperties rendprops)
    {
        NodeManager.SaveAllAsTemplate(name, rendprops);
        NodeManager.ClearAll();
        LoadHUD();
    }

    public void SaveAsNewProject(string name)
    {
        CurrentProjectID = NodeManager.SaveAsNewProject(name);
    }

    public void SaveChanges()
    {
        NodeManager.SaveChangesToProject(CurrentProjectID);
        Debug.Log($"Changes saved to project {CurrentProjectName}");
    }






    public void _dbg_OnLoadProject()
    {
        //[TODO] clear or save curent project!!
        Debug.Assert(AppSaveData.Projects.Count > 0);
        AppSaveData.Projects.Last().BuildProjectFromTemplate(new Vector2(2, 5));
        //[TODO]
    }



    /* Stany i zmienne potrzebne do ró¿nych stanów: */

    private State PlayerState;
    public enum State
    {
        Idle,
        NodeDrag,
        EdgeNew,
        EdgeOld,
        NodeNew,
    }

    private void StateIdleStart()
    {
        PlayerState = State.Idle;
    }
    private void StateIdle()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var obj = GetObjectUnderMouse();
            //Debug.Log(EventSystem.current.currentSelectedGameObject);
            if (obj == null)
                ContextMenu.Instance.DestroyContextMenu();
            if (obj != null)
            {
                Debug.Log("clicked " + GetObjectUnderMouse());
                ChangeSelectionTo(obj.GetComponent<CollisionData>());

                if (selectedObject != null)
                    ContextMenu.Instance.DestroyContextMenu();
                //InvokeNextFrame(ContextMenu.Instance.DestroyContextMenu);

                if (selectedObject is NodeCollision)
                {
                    selectedNode = (selectedObject as NodeCollision).node;
                    StateMachine.ChangeState(new StateMachine.PlayerState(StateNodeInteract));
                }
                else if (selectedObject is OutSocketCollision)
                {
                    var info = selectedObject as OutSocketCollision;
                    edgeNewRend = EdgeRenderer.Make(info.sourceNode, info.outIdx, GetMousePosition());
                    StateMachine.ChangeState(new StateMachine.PlayerState(StateEdgeNew));
                }
                else if (selectedObject is InSocketCollision)
                {
                    inSocket = selectedObject as InSocketCollision;
                    StateMachine.ChangeState(new StateMachine.PlayerState(StateEdgeOld));
                }
                else if (selectedObject is NodeTemplateCollision)
                {
                    //...
                    newNodeTemplateID = (selectedObject as NodeTemplateCollision).TemplateID;
                    StateMachine.ChangeState(new StateMachine.PlayerState(StateNodeNew));
                }
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            var obj = GetObjectUnderMouse();
            if (obj != null)
            {
                Debug.Log("[TODO] Tutaj bedzie menu kontekstowe -> opcje danej bramki/krawedzi...");
            }
        }
        else if (Input.GetMouseButtonDown(2))
        {
            StateMachine.ChangeState(StateCameraPan);
        }
    }
    private void StateIdleEnd()
    {

    }

    private CollisionData selectedObject;
    private Node selectedNode;





    private void StateCameraPanStart()
    {
        ContextMenu.Instance.DestroyContextMenu();
        mousePosStart = GetMousePositionConstCamera();
        cameraPosStart = m_Camera.transform.position;
    }
    private void StateCameraPan()
    {
        if (!Input.GetMouseButton(2))
        {
            StateMachine.ChangeState(StateIdle);
        }
        else
        {
            var delta = GetMousePositionConstCamera() - mousePosStart;
            Debug.Log(delta);
            m_Camera.transform.position = cameraPosStart - (Vector3)delta;//reverse
        }
    }
    private void StateCameraPanEnd()
    {
    }
    Vector2 mousePosStart;
    Vector3 cameraPosStart;






    private void StateNodeNewStart()
    {
        PlayerState = State.NodeNew;
        selectedNode = NodeManager.CreateNode(AppSaveData.GetTemplate(newNodeTemplateID), GetMousePosition());
        ChangeSelectionTo(selectedNode.GetRenderer().GetComponent<NodeCollision>());
        //Debug.Assert(false);
        selectedNode.GetRenderer().MoveOverUI();
    }
    private void StateNodeNew()
    {
        Vector2 newPos = GetMousePosition();
        if (AppSaveData.Settings.SnapObjects != Input.GetKey(KeyCode.LeftControl))//albo albo
        {
            //wtedy snapujemy do siatki
            float dist = AppSaveData.Settings.SnapDistance;
            newPos.x = Mathf.Round(newPos.x / dist) * dist;
            newPos.y = Mathf.Round(newPos.y / dist) * dist;
            Debug.Log(newPos);
        }
        selectedNode.Position = newPos;

        if (!Input.GetMouseButton(0))
        {
            if (IsPointerOverUIObject())
            {
                //usuwamy
                NodeManager.DeleteNode(selectedNode);
            }
            else
            {
                //git, tylko przesuwamy z powrotem
                selectedNode.GetRenderer().MoveBehindUI();
            }
            StateMachine.ChangeState(new StateMachine.PlayerState(StateIdle));
        }
    }
    private void StateNodeNewEnd()
    {
    }

    //Node selectedNode;
    //bool startedDragging;
    private int newNodeTemplateID;





    private void StateEdgeNewStart()
    {
        PlayerState = State.EdgeNew;
    }
    private void StateEdgeNew()
    {
        edgeNewRend.End = GetMousePosition();
        //[TODO] dorobiæ layermask tutaj, ¿eby wykrywa³o tylko collidery socketów 

        var obj = GetObjectUnderMouse();
        InSocketCollision currentInputSocket = null;
        if (obj != null)
            currentInputSocket = obj.GetComponent<InSocketCollision>();

        //[TODO] ewentualnie dorobiæ podgl¹d wartoœci, gdyby po³¹czono z tym socketem

        if (!Input.GetMouseButton(0))
        {
            edgeNewRend.Destroy();
            if (currentInputSocket != null)
            {
                if (edgeNewRend.from != currentInputSocket.targetNode)
                {
                    //czy przypadkiem nie ³¹czymy z tym samym
                    NodeManager.Connect(edgeNewRend.from, edgeNewRend.outIdx,
                        currentInputSocket.targetNode, currentInputSocket.inIdx);
                }
            }
            StateMachine.ChangeState(new StateMachine.PlayerState(StateIdle));
        }
    }
    private void StateEdgeNewEnd()
    {
    }

    EdgeRenderer edgeNewRend;





    private void StateEdgeOldStart()
    {
        PlayerState = State.EdgeOld;
    }
    private void StateEdgeOld()
    {
        if (!Input.GetMouseButton(0))
        {
            StateMachine.ChangeState(new StateMachine.PlayerState(StateIdle));
            return;
        }

        //[DESIGN] jesli usuniemy ten próg od³¹czenia socketu (0.2f), 
        // to wgl bêdzie mo¿na usun¹æ ca³y stan EdgeOld
        var mpos = GetMousePosition();
        if (Vector2.Distance(mpos, inSocket.transform.position) > 0.2f)
        {
            var (node, inidx) = (inSocket.targetNode, inSocket.inIdx);
            var (from, outidx) = (node.ins[inidx].st, node.ins[inidx].nd);
            if (from == null)
            {
                StateMachine.ChangeState(new StateMachine.PlayerState(StateIdle));
            }
            else
            {
                NodeManager.Disconnect(from, outidx, node, inidx);

                edgeNewRend = EdgeRenderer.Make(from, outidx, mpos);
                StateMachine.ChangeState(new StateMachine.PlayerState(StateEdgeNew));
            }
        }
    }
    private void StateEdgeOldEnd()
    {
    }

    InSocketCollision inSocket;





    private void StateNodeInteractStart()
    {
        PlayerState = State.NodeDrag;
        mousePositionLMBDown = GetMousePosition();
        Debug.Assert(selectedNode != null);
        selectedNodePositionLMBDown = selectedNode.Position;
        startedDragging = false;

        controller = null;

        if (selectedNode is InputNode inputNode && inputNode.Controlled)
        {
            controller = inputNode.Controller;
            controllerPositionLMBDown = controller.Position;
        }
        else if (selectedNode is OutputNode outputNode && outputNode.Controlled)
        {
            controller = outputNode.Controller;
            controllerPositionLMBDown = controller.Position;
        }
    }
    private void StateNodeInteract()
    {
        var deltaPosition = GetMousePosition() - mousePositionLMBDown;
        if (startedDragging || deltaPosition.magnitude > 0.2f) //zeby przez przypadek 
        {                                                       //nie przesuwaæ o 0.001mm
            startedDragging = true; //Debug.Log("draggin"); 
            if (controller != null)
            {
                ChangeSelectionTo(controller.GetRenderer().GetComponent<CollisionData>());
                selectedNode = controller;//chyba git
                selectedNodePositionLMBDown = controllerPositionLMBDown;
                controller = null;
            }

            Vector2 newPos = selectedNodePositionLMBDown + deltaPosition;
            if (AppSaveData.Settings.SnapObjects != Input.GetKey(KeyCode.LeftControl))//albo albo
            {
                //wtedy snapujemy do siatki
                float dist = AppSaveData.Settings.SnapDistance;
                newPos.x = Mathf.Round(newPos.x / dist) * dist;
                newPos.y = Mathf.Round(newPos.y / dist) * dist;
                //Debug.Log(newPos);
            }
            selectedNode.Position = newPos;
        }

        if (!Input.GetMouseButton(0)) //jesli juz nie trzymamy LPM
        {
            if (!startedDragging && selectedObject is InputCollision)
            {
                NodeManager.Flip(selectedObject as InputCollision);
            }
            StateMachine.ChangeState(new StateMachine.PlayerState(StateIdle));
        }
    }
    private void StateNodeInteractEnd()
    {

    }

    bool startedDragging;
    Vector2 mousePositionLMBDown;
    MultibitController controller;
    Vector2 controllerPositionLMBDown;
    Vector2 selectedNodePositionLMBDown;





    private void State_NAZWA_Start()
    {
    }
    private void State_NAZWA_()
    {
    }
    private void State_NAZWA_End()
    {
    }





    /* Pomocnicze funkcje odsyfiaj¹ce resztê kodu: */
    public void LoadHUD()
    {
        for (int i = 0; i < BottomBarContent.transform.childCount; i++)
        {
            Destroy(BottomBarContent.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < AppSaveData.TemplateCnt; i++)
        {
            var template = AppSaveData.GetTemplate(i);
            var coll = Instantiate(Resources.Load<GameObject>("Sprites/UI/Template Klocka"), BottomBarContent.transform).GetComponent<NodeTemplateCollision>();
            coll.TemplateID = i;
            coll.Color = template.renderProperties.Color;
            coll.NodeName = template.defaultName;
        }
    }
    private void ChangeSelectionTo(CollisionData curr)
    {
        var prev = selectedObject;
        if (prev == curr) return;
        Debug.Log($"Selected obj is now {curr}");
        if (prev != null) prev.Renderer.Selected = false;
        if (curr != null) curr.Renderer.Selected = true;
        selectedObject = curr;
    }
    private Vector2 GetMousePosition()
    {
        return m_Camera.ScreenToWorldPoint(Input.mousePosition);
    }
    private Vector2 GetMousePositionConstCamera()
    {
        return c_Camera.ScreenToWorldPoint(Input.mousePosition);
    }
    private GameObject GetObjectUnderMouse()
    {
        //(C)
        if (EventSystem.current.currentSelectedGameObject != null)
            return EventSystem.current.currentSelectedGameObject;

        var origin = GetMousePosition();//czy nie vector3??[!!!!]
        var hit = Physics2D.Raycast(origin, Vector2.zero);
        if (hit.collider != null)
            return hit.collider.gameObject;//return hit.collider != null ? hit.collider.gameObject : null;
        return null;

        //(A)
        //return EventSystem.current.currentSelectedGameObject;

        //(B)
        //if(hit.collider != null) return hit.collider.gameObject;

        //var pointerData = new PointerEventData(EventSystem.current);
        //List<RaycastResult> results = new List<RaycastResult>();
        //EventSystem.current.RaycastAll(pointerData, results);

        //if (results.Count > 0)
        //{
        //    Debug.Log($"first of {results.Count} elements: {results[0]}");
        //    return results[0].gameObject;
        //}
        //return null;
    }
    public static bool IsPointerOverUIObject()
    {
        //[TODO] sprawdzic czy to nie jest ultra smooth
        PointerEventData eventDataCurrentPosition = new(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    public void InvokeNextFrame(Action function)
    {
        try
        {
            StartCoroutine(_InvokeNextFrame(function));
        }
        catch
        {
            Debug.Log("Trying to invoke " + function.ToString() + " but it doesnt seem to exist");
        }
    }
    private IEnumerator _InvokeNextFrame(Action function)
    {
        yield return null;
        function();
    }

    [SerializeField]
    private GameObject BottomBarContent;
    private int currentProjectID = -1;
}
