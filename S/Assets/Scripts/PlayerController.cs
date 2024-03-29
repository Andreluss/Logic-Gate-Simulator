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

    public enum GameMode
    {
        Menu, Normal, Edit
    }

    internal void SwitchToEditMode(int id)
    {
        CurrentlyEditedBlockID = id;
        Mode = GameMode.Edit;
    }
    internal void SwitchBackToNormalMode()
    {
        Mode = GameMode.Normal;
    }


    private GameMode mode = GameMode.Normal;


    /* okienka */
    [SerializeField]
    private GameObject NormalUI;
    [SerializeField]
    private GameObject EditUI;

    [SerializeField]
    private GameObject Menu;
    [SerializeField]
    private GameObject SaveAsNewBlockMenu;
    [SerializeField]
    private GameObject SaveOnExitMenu;
    [SerializeField]
    private GameObject SaveOnExitEditModeMenu;//!
    [SerializeField]
    private GameObject SaveAsNewProjectMenu;

    [SerializeField]
    private GameObject ChangeColorMenu;
    [SerializeField]
    private GameObject ChangeDescriptionMenu;

    [SerializeField]
    private GameObject EMSaveOnExitMenu;
    [SerializeField]
    private TextMeshProUGUI EMInfo;

    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private GameObject BottomBarContent;
    [SerializeField]
    private GameObject HideTemplateMenu;
    [SerializeField]
    private GameObject HideTemplateFromProjectMenu;
    [SerializeField]
    private GameObject DeleteProjectMenu;

    [HideInInspector]
    public ScreenShotManager screenShotManager;

    private Button sampleButton;

    //[SerializeField]
    //private GameObject InputPanel;
    //private RectTransform Irect;
    //[SerializeField]
    //private GameObject OutputPanel;
    //private RectTransform Orect;


    /* Dane aktualnego projektu */
    public int CurrentProjectID { get => currentProjectID; set => currentProjectID = value; }
    public int CurrentlyEditedBlockID { get; set; }
    public List<int> GatesInCurrentProject = new() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
    public string CurrentProjectName { get => CurrentProjectID == -1 ? "Untitled" : AppSaveData.GetProject(CurrentProjectID).defaultName;}
    public string CurrentlyEditedBlockName { get => AppSaveData.GetTemplate(CurrentlyEditedBlockID).defaultName; }
    public RenderProperties CurrentlyEditedBlockRendProps { get => AppSaveData.GetTemplate(CurrentlyEditedBlockID).renderProperties; }

    public GateTemplate TemporaryProjectSave { get; private set; }
    public GameMode Mode
    {
        get => mode; set
        {
            if(mode == value) return;

            //destruct
            switch (mode)
            {
                case GameMode.Menu:
                    break;

                case GameMode.Normal:
                    ShowNormalUI(false);
                    break;

                case GameMode.Edit:
                    NodeManager.ClearAll();
                    ShowEditUI(false);
                    CurrentlyEditedBlockID = -1;
                    break;
            }

            mode = value;

            //construct
            switch (value)
            {
                case GameMode.Menu:
                    NodeManager.ClearAll();
                    ShowMenu();
                    TemporaryProjectSave = null;//??
                    break;

                case GameMode.Normal:
                    ShowNormalUI(true);
                    bool changes = NodeManager.UnsavedChangesInProject;
                    if (TemporaryProjectSave != null)
                    {
                        TemporaryProjectSave.BuildProjectFromTemplate();
                        TemporaryProjectSave = null;
                    }
                    NodeManager.UnsavedChangesInProject = changes;
                    ReloadHUD();
                    break;

                case GameMode.Edit:
                    TemporaryProjectSave = NodeManager.GetProjectSaveFromAll("Last opened project's state");
                    NodeManager.ClearAll();

                    ShowEditUI(true); Debug.Assert(CurrentlyEditedBlockID != -1);
                    ReloadHUD();
                    var t = AppSaveData.GetTemplate(CurrentlyEditedBlockID);
                    t.BuildProjectFromTemplate(); //load block as project !! [BUG??]
                    NodeManager.UnsavedChanges = false;

                    string html_col = ColorUtility.ToHtmlStringRGB(t.renderProperties.Color);
                    EMInfo.text = $"Editing block: <color=#{html_col}>{t.defaultName}</color>";

                    break;
            }
        }
    }



    /* Funkcje zwi�zane z trybem Gry (tryb >> stan) */
    private void ShowNormalUI(bool show)
    {
        NormalUI.SetActive(show);
        BottomBarContent.SetActive(show);
    }
    private void ShowEditUI(bool show)
    {
        EditUI.SetActive(show);
        BottomBarContent.SetActive(show);
    }
    private void ShowMenu()
    {
        Instantiate(Menu, canvas.transform);
    }

    private void ShowEMSaveOnExitMenu()
    {
        Instantiate(EMSaveOnExitMenu, canvas.transform);
    }


    /*     === Unity funkcje ===     */
    private void Awake()
    {
        m_Camera = Camera.main;
        c_Camera = GameObject.Find("Const Camera").GetComponent<Camera>();

        screenShotManager = GetComponent<ScreenShotManager>();

        AppSaveData.Load();
        ReloadHUD();
        StateMachine = GetComponent<StateMachine>();
        StateMachine.Initialize(new StateMachine.PlayerState(StateIdle));
        PlayerState = State.Idle;

        PlayerController.Instance.Mode = PlayerController.GameMode.Menu;
        NodeManager.UnsavedChangesInBlock = false;
        NodeManager.UnsavedChangesInProject = false;

        //Irect = InputPanel.GetComponent<RectTransform>();
        //Orect = OutputPanel.GetComponent<RectTransform>();
        RecalcIOPanels();
        sampleButton = Resources.Load<Button>("Sprites/UI/ContextButton");
        //UnityEngine.Assertions.Assert.raiseExceptions = true;
    }




    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.K))
        //{
        //    screenShotManager.SaveScreenShot(CurrentProjectID);
        //}


        StateMachine.UpdateStateMachine();
        if (PlayerState == State.Idle)
        {
            //wkurza mnie ten warning
            //Debug.Log("is over UI: " + IsPointerOverUIObject());
        }
        if(EventSystem.current.IsPointerOverGameObject() == false && Mode != GameMode.Menu && !IsPointerOutsideTheViewport())
        {
            var delta = -Input.mouseScrollDelta.y;
            if (Mathf.Abs(delta) > 0)
            {
                Debug.Log(Input.mouseScrollDelta);
                var mouse1 = GetMousePosition();
                m_Camera.orthographicSize = Mathf.Clamp(m_Camera.orthographicSize + delta * 0.75f, 1f, 100000f);
                var mouse2 = GetMousePosition();
                m_Camera.transform.position += (Vector3)(mouse1 - mouse2);

                c_Camera.orthographicSize = m_Camera.orthographicSize;

                ContextMenu.Instance.DestroyContextMenu();

                //var pos = GetMousePosition();
                //Vector3 newpos = new Vector3(pos.x, pos.y, m_Camera.transform.position.z);

                //m_Camera.transform.position = newpos;


                if (AppSaveData.Settings.PinInOutToScreenEdges)
                {
                    NodeManager.PinAll();
                    RecalcIOPanels();
                }
            }
        }
    }

    //private float _deb_ = 880f;
    //private void _RecalcIOPanel(RectTransform rt)
    //{
    //    var size = Irect.sizeDelta;
    //    size.x = _deb_ / m_Camera.orthographicSize;
    //    rt.sizeDelta = size;
    //}
    private void RecalcIOPanels()
    {
        //_RecalcIOPanel(Irect);
        //_RecalcIOPanel(Orect);
    }


    /* Klikni�cia guzik�w */
    public void OnTogglePinInOut()
    {
        AppSaveData.Settings.PinInOutToScreenEdges = !AppSaveData.Settings.PinInOutToScreenEdges;
        if(AppSaveData.Settings.PinInOutToScreenEdges)
        {
            //InputPanel.SetActive(true);
            //OutputPanel.SetActive(true);
            //RecalcIOPanels();

            NodeManager.PinAll();
        }
        else
        {
            //InputPanel.SetActive(false);
            //OutputPanel.SetActive(false);
        }
        
    }
    public void OnToggleAllGates()
    {
        AppSaveData.Settings.ShowAllGates = !AppSaveData.Settings.ShowAllGates;
        ReloadHUD();
    }
    public void OnToggleDescriptions()
    {
        //ahh bo Unity nie widzi menegera
        NodeManager.ToggleDestriptions();
    }
    public void OnToggleSnapping()
    {
        AppSaveData.Settings.SnapObjects = !AppSaveData.Settings.SnapObjects;
        //_SnapToGrid.isOn = AppSaveData.Settings.SnapObjects;
    }
    public void OnToggleFullscreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
        if(Screen.fullScreen)
        {
            var resMax = Screen.resolutions.Last();
            Screen.SetResolution(resMax.width, resMax.height, true);
        }

        Debug.Log(Screen.resolutions);
        Debug.Log($"FULLSCREEN {Screen.fullScreen}");
    }
    public void OnClear()
    {
        CurrentPopUpWindow = null;
        ContextMenu.Instance.DestroyContextMenu();
        NodeManager.ClearAll();

        NodeManager.UnsavedChanges = true;
    }
    //public void OnTypeValue(MultibitController controller)
    //{
    //    throw new System.NotImplementedException();
    //}

    // NormalMode
    public void OnSaveClick(bool andClose) //(Ctrl + S)
    {
        if (CurrentProjectID == -1)
        {
            //save as new project or cancel
            ShowSaveAsNewProjectMenu(andClose);
        }
        else
        {
            //just save changes
            SaveChanges();
            screenShotManager.SaveScreenShot(CurrentProjectID);
            if (andClose)
            {
                NodeManager.ClearAll();
                Mode = GameMode.Menu;
            }
        }
    }
    public void OnExitClick()
    {
        if(NodeManager.UnsavedChanges)
        {
            //Debug.LogWarning("unsaved in proj: " + NodeManager.UnsavedChangesInProject);
            ShowSaveOnExitMenu();
        }//OK sprawdzic, czy s� wgl zmiany do zapisania!!
        else {
            PlayerController.Instance.Mode = PlayerController.GameMode.Menu;
            NodeManager.UnsavedChangesInBlock = false;
            NodeManager.UnsavedChangesInProject = false;
        }
    }


    // EditMode (EM)
    public void OnEMSaveClick(bool andExitToNormal = false)
    {
        SaveEMChanges();
        if(andExitToNormal)
        {
            Mode = GameMode.Normal;
        }
    }

    public void OnEMExitClick()
    {
        if(NodeManager.UnsavedChanges)
        {
            //Debug.LogWarning("unsaved in block: " + NodeManager.UnsavedChangesInBlock);
            ShowEMSaveOnExitMenu();
        }
        else
        {
            PlayerController.Instance.Mode = PlayerController.GameMode.Normal;
            NodeManager.UnsavedChangesInBlock = false;
        }
    }







    /* wy�wietlacze menu itp. */
    public GameObject CurrentPopUpWindow
    {
        get => currentPopUpWindow; set
        {
            if(currentPopUpWindow != null)
                Destroy(currentPopUpWindow);
            currentPopUpWindow = value;
        }
    }


    private GameObject currentPopUpWindow;
    public void ShowSaveAsNewBlockMenu()
    {
        Instantiate(SaveAsNewBlockMenu, canvas.transform);
    }
    public void ShowSaveAsNewProjectMenu(bool andClose = false)
    {
        var savemenu = Instantiate(SaveAsNewProjectMenu, canvas.transform);
        savemenu.GetComponentInChildren<SaveAsNewProjectMenuController>().SaveAndClose = andClose;
        //troche syf no ale nwm jak inaczej to przes�a� sprytnie
    }
    public void ShowSaveOnExitMenu()
    {
        Instantiate(SaveOnExitMenu, canvas.transform);
    }
    public void ShowChangeColorMenu(GateRenderer forGateRenderer)
    {
        var menu = Instantiate(ChangeColorMenu, forGateRenderer.node.Position + new Vector2(1.25f, 0), Quaternion.identity, canvas.transform);
        menu.GetComponentInChildren< ChangeColorMenuController>().gateRenderer = forGateRenderer;

        CurrentPopUpWindow = menu;
    }
    public void ShowChangeDescriptionMenu(Node inoutnode)
    {
        var menu = Instantiate(ChangeDescriptionMenu, inoutnode.Position + new Vector2(1f, 0), Quaternion.identity, canvas.transform);
        menu.GetComponentInChildren<ChangeDescriptionMenuController>().node = inoutnode;

        CurrentPopUpWindow = menu;
    }

    public void ShowHideTemplateMenu(int id)
    {
        var menu = Instantiate(HideTemplateMenu, canvas.transform);
        menu.transform.GetChild(0).GetComponent<HideTemplateMenuController>().WhatTemplate = id;
    }

    internal void ShowHideTemplateFromProjectMenu(int id)
    {
        var menu = Instantiate(HideTemplateFromProjectMenu, canvas.transform);
        menu.transform.GetChild(0).GetComponent<HideTemplateFromProjectMenuController>().WhatTemplate = id;
    }

    internal void ShowDeleteProjectMenu(int id)
    {
        var menu = Instantiate(DeleteProjectMenu, canvas.transform);
        menu.transform.GetChild(0).GetComponent<DeleteProjectMenuController>().WhatProject = id;
    }

    /* w�a�ciwe operacje ju� po wyklikaniu opcji z menu */
    public void HideTemplateForever(int id)
    {
        AppSaveData.HideTemplate(id);
        if (Mode == GameMode.Edit)
            ReloadHUD();
        else
            ReloadHUD();
    }
    public void RemoveTemplateFromProjectList(int id)
    {
        GatesInCurrentProject.Remove(id);
        NodeManager.UnsavedChangesInProject = true;
        ReloadHUD();
    }
    public void SaveAllAsNewTemplate(string name, RenderProperties rendprops)
    {
        GatesInCurrentProject.Add(NodeManager.SaveAllAsNewTemplate(name, rendprops).templateId);
        NodeManager.UnsavedChangesInProject = true;//
        NodeManager.ClearAll();
        Debug.Log($"New block ({name}) has been succesfully saved.");
        ReloadHUD();
    }
    public void SaveAsNewProject(string name, bool andClose)
    {
        CurrentProjectID = NodeManager.SaveAllAsNewProject(name);
        Debug.Log($"New project ({name}) has been succesfully saved.");
        if (andClose)
        {
            NodeManager.ClearAll();
            Mode = GameMode.Menu;
        }
    }

    /// <summary>
    /// �aduje zapisany projekt (lub nowy, je�li id = -1)
    /// </summary>
    /// <param name="id">id projektu do za�adowania</param>
    public void LoadProject(int id)
    {
        NodeManager.ClearAll();
        CurrentProjectID = id;
        
        if (id == -1)
        {
            NodeManager.UnsavedChangesInProject = true;
            GatesInCurrentProject = new() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            ReloadHUD();
        }
        else
        {
            NodeManager.UnsavedChangesInProject = false;
        
            var pt = AppSaveData.GetProject(id);
            GatesInCurrentProject = pt.gatesAvailableInThisProject; 
            AppSaveData.GetProject(id).BuildProjectFromTemplate();
            ReloadHUD();
        }

        if (AppSaveData.Settings.PinInOutToScreenEdges)
            NodeManager.PinAll();
    }

    public void SaveChanges()
    {
        NodeManager.SaveChangesToProject(CurrentProjectID);
        Debug.Log($"Changes saved to project {CurrentProjectName}");
    }
    private void SaveEMChanges()
    {
        Debug.Assert(CurrentlyEditedBlockID != -1);
        NodeManager.SaveChangesToTemplate(CurrentlyEditedBlockID);
        Debug.Log($"Changes saved to edited block");
    }

    //public bool UnsavedChanges()
    //{

    //    if (Mode == GameMode.Normal)
    //    {
    //        var t = NodeManager.GetProjectSaveFromAll(CurrentProjectName);
    //        if(AppSaveData.GetProject(currentProjectID)
    //    }
    //    else if(Mode == GameMode.Edit)
    //    {
    //        var t = NodeManager.GetBlockSaveFromAll(CurrentlyEditedBlockName, CurrentlyEditedBlockRendProps);
    //    }
    //}


    /* Stany i zmienne potrzebne do r�nych stan�w: */

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
        //handle selection
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            var obj = GetObjectUnderMouse();
            if (obj == null)
            {
                ContextMenu.Instance.DestroyContextMenu();

                if(Mode == GameMode.Normal && Input.GetMouseButtonDown(1))
                {
                    float x_left = m_Camera.ScreenToWorldPoint(new Vector3(Screen.width / 7f, 0, 0)).x;
                    float x_right = m_Camera.ScreenToWorldPoint(new Vector3(Screen.width * 6f/7f, 0, 0)).x;

                    Vector2 pos = GetMousePosition();

                    if(pos.x < x_left)
                    {
                        List<ContextMenuItem> items = new();
                        items.Add(new ContextMenuItem("Create 1Bit Input", sampleButton,
                            () => NodeManager.CreateNode(AppSaveData.GetTemplate(0), pos)));
                        items.Add(new ContextMenuItem("Create 2Bit Input", sampleButton,
                            () => NodeManager.CreateNode(AppSaveData.GetTemplate(1), pos)));
                        items.Add(new ContextMenuItem("Create 4Bit Input", sampleButton,
                            () => NodeManager.CreateNode(AppSaveData.GetTemplate(2), pos)));
                        items.Add(new ContextMenuItem("Create 8Bit Input", sampleButton,
                            () => NodeManager.CreateNode(AppSaveData.GetTemplate(3), pos)));
                        ContextMenu.Instance.CreateContextMenu(items, new Vector2(pos.x, pos.y));
                    }
                    else if(x_right < pos.x)
                    {
                        List<ContextMenuItem> items = new();
                        items.Add(new ContextMenuItem("Create 1Bit Output", sampleButton,
                            () => NodeManager.CreateNode(AppSaveData.GetTemplate(0), pos)));
                        items.Add(new ContextMenuItem("Create 2Bit Output", sampleButton,
                            () => NodeManager.CreateNode(AppSaveData.GetTemplate(1), pos)));
                        items.Add(new ContextMenuItem("Create 4Bit Output", sampleButton,
                            () => NodeManager.CreateNode(AppSaveData.GetTemplate(2), pos)));
                        items.Add(new ContextMenuItem("Create 8Bit Output", sampleButton,
                            () => NodeManager.CreateNode(AppSaveData.GetTemplate(3), pos)));
                        ContextMenu.Instance.CreateContextMenu(items, new Vector2(pos.x, pos.y));
                    }
                }


                ChangeSelectionTo(null);//[BUG?]
            }
            else if (obj != null)
            {
                Debug.Log("clicked " + GetObjectUnderMouse());
                ChangeSelectionTo(obj.GetComponent<CollisionData>());
            }

            if (Input.GetMouseButtonDown(0))
            {
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
            if (AppSaveData.Settings.PinInOutToScreenEdges)
            {
                NodeManager.PinAll();
            }

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
        NodeManager.UnsavedChanges = true;

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

                if (AppSaveData.Settings.PinInOutToScreenEdges) 
                    selectedNode.GetRenderer().HandlePinPosition();

                //aktualizujemy bramki w aktualnym projekcie
                if(/*Mode == GameMode.Normal && */!GatesInCurrentProject.Contains(newNodeTemplateID))
                {
                    GatesInCurrentProject.Add(newNodeTemplateID);
                    GatesInCurrentProject.Sort();
                }

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
        //[TODO] dorobi� layermask tutaj, �eby wykrywa�o tylko collidery socket�w 

        var obj = GetObjectUnderMouse();
        InSocketCollision currentInputSocket = null;
        if (obj != null)
            currentInputSocket = obj.GetComponent<InSocketCollision>();

        //[TODO] ewentualnie dorobi� podgl�d warto�ci, gdyby po��czono z tym socketem

        if (!Input.GetMouseButton(0))
        {
            edgeNewRend.Destroy();
            if (currentInputSocket != null)
            {
                if (edgeNewRend.from != currentInputSocket.targetNode)
                {
                    //czy przypadkiem nie ��czymy z tym samym
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

        //[DESIGN] jesli usuniemy ten pr�g od��czenia socketu (0.2f), 
        // to wgl b�dzie mo�na usun�� ca�y stan EdgeOld
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
        {                                                       //nie przesuwa� o 0.001mm
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
            
            selectedNode.Position = newPos;//nodemanager.move()?
            
            if (AppSaveData.Settings.PinInOutToScreenEdges)
                selectedNode.GetRenderer().HandlePinPosition();

            NodeManager.UnsavedChanges = true;

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





    /* Pomocnicze funkcje odsyfiaj�ce reszt� kodu: */
    private void MakeTemplateKlocka(GateTemplate t)
    {
        var coll = Instantiate(Resources.Load<GameObject>("Sprites/UI/Template Klocka"), BottomBarContent.transform).GetComponent<NodeTemplateCollision>();
        coll.TemplateID = t.templateId;
        coll.Color = t.renderProperties.Color;
        coll.NodeName = t.defaultName;
    }
    public void ReloadHUD()
    {
        var endGateID = int.MaxValue;
        if (Mode == GameMode.Edit)
            endGateID = CurrentlyEditedBlockID;

        for (int i = 0; i < BottomBarContent.transform.childCount; i++)
        {
            Destroy(BottomBarContent.transform.GetChild(i).gameObject);
        }
        if(AppSaveData.Settings.ShowAllGates)
        {
            for (int i = (endGateID == int.MaxValue ? 0 : 8); i < Mathf.Min(AppSaveData.TemplateCnt, endGateID); i++)
            {
                var template = AppSaveData.GetTemplate(i);
                if (template.DELETED) continue;
                MakeTemplateKlocka(template);
            }
        }
        else //tylko bramki z tego projektu
        {
            List<int> gates;
            if(currentProjectID == -1)
                gates = new() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            else 
                gates = AppSaveData.GetProject(currentProjectID).gatesAvailableInThisProject;

            
            if(GatesInCurrentProject != null && GatesInCurrentProject.Count > 10)
                gates = GatesInCurrentProject;

            //Debug.Assert(gates.SequenceEqual(GatesInCurrentProject));

            var startGate = (Mode == GameMode.Normal ? 0 : 8);
            foreach (var gateID in gates)
            {
                if (gateID < startGate) continue;
                if (!(gateID < endGateID)) break;
                var gateTemplate = AppSaveData.GetTemplate(gateID);
                if(gateTemplate.DELETED) continue;

                MakeTemplateKlocka(gateTemplate);
            }
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
    public bool IsPointerOutsideTheViewport()
    {
        var view = m_Camera.ScreenToViewportPoint(Input.mousePosition);
        var isOutside = view.x < 0 || view.x > 1 || view.y < 0 || view.y > 1;
        return isOutside;
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


    private int currentProjectID = -1;
}
