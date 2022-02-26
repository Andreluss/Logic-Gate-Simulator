using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(StateMachine))]
public class PlayerController : MonoBehaviour
{
    private Camera m_Camera;
    private StateMachine StateMachine;
    

    private void Awake()
    {
        m_Camera = Camera.main; 
        AppSaveData.Load();
        LoadHUD();
        StateMachine = GetComponent<StateMachine>();
        StateMachine.Initialize(new StateMachine.PlayerState(StateIdle));
        PlayerState = State.Idle;

        //UnityEngine.Assertions.Assert.raiseExceptions = true;
    }

    void Start()
    {
        
    }
    


    void Update()
    {
        StateMachine.UpdateStateMachine();
        if(PlayerState == State.Idle)
        {
            //wkurza mnie ten warning
            //Debug.Log("is over UI: " + IsPointerOverUIObject());
        }
        //if (EventSystem.current.currentSelectedGameObject != null)
        //    Debug.Log(EventSystem.current.currentSelectedGameObject);
    }

    private void FixedUpdate()
    {

    }

    /* Klikni�cia guzik�w */
    public void OnSaveAsTemplateClick()
    {
        //StateMachine.ChangeState(new StateMachine.PlayerState(StateGateNew));
        NodeManager.SaveAllAsTemplate("NAND", new RenderProperties(Color.magenta, Vector2.zero));
    }




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
        if (Input.GetMouseButtonDown(0))
        {
            var obj = GetObjectUnderMouse();
            Debug.Log(EventSystem.current.currentSelectedGameObject);
            if (obj != null)
            {
                Debug.Log("clicked " + GetObjectUnderMouse().name);
                ChangeSelectionTo(obj.GetComponent<CollisionData>());


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
                else if(selectedObject is InSocketCollision)
                {
                    inSocket = selectedObject as InSocketCollision;
                    StateMachine.ChangeState(new StateMachine.PlayerState(StateEdgeOld));
                }
                else if(selectedObject is NodeTemplateCollision)
                {
                    //...
                    newNodeTemplateID = (selectedObject as NodeTemplateCollision).TemplateID;
                    StateMachine.ChangeState(new StateMachine.PlayerState(StateNodeNew));
                }
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            var obj = GetObjectUnderMouse();
            if(obj != null)
            {
                Debug.Log("[TODO] Tutaj bedzie menu kontekstowe -> opcje danej bramki/krawedzi...");
            }
        }
    }
    private void StateIdleEnd()
    {

    }

    private CollisionData selectedObject;
    private Node selectedNode;



    
    
    private void StateGateNewStart()
    {
    }
    private void StateGateNew()
    {
    }
    private void StateGateNewEnd()
    {
    }





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
            if(IsPointerOverUIObject())
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
        //[TODO] dorobi� layermask tutaj, �eby wykrywa�o tylko collidery socket�w 
        
        var obj = GetObjectUnderMouse();
        InSocketCollision currentInputSocket = null;
        if (obj != null) 
            currentInputSocket = obj.GetComponent<InSocketCollision>();
        
        //[TODO] ewentualnie dorobi� podgl�d warto�ci, gdyby po��czono z tym socketem

        if(!Input.GetMouseButton(0))
        {
            edgeNewRend.Destroy();
            if (currentInputSocket != null)
            {
                if(edgeNewRend.from != currentInputSocket.targetNode)
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
        if(Vector2.Distance(mpos, inSocket.transform.position) > 0.2f)
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
        else if(selectedNode is OutputNode outputNode && outputNode.Controlled)
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
            if(controller != null)
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
            if(!startedDragging && selectedObject is InputCollision)
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
    public void ToggleDescriptions()
    {
        //ahh bo Unity nie widzi menegera
        NodeManager.ToggleDestriptions();
    }
    [SerializeField]
    private GameObject BottomBarContent;
    private void LoadHUD()
    {
        for (int i = 0; i < AppSaveData.TemplateCnt; i++)
        {
            var button = Instantiate(Resources.Load<GameObject>("Sprites/UI/Template Klocka"), BottomBarContent.transform);
            button.GetComponent<NodeTemplateCollision>().TemplateID = i;
            button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = AppSaveData.GetTemplate(i).defaultName;
        }
    }
    private void ChangeSelectionTo(CollisionData curr)
    {
        var prev = selectedObject;
        if(prev == curr) return;
        Debug.Log($"Selected obj is now {curr}");
        if (prev != null) prev.Renderer.Selected = false;
        if (curr != null) curr.Renderer.Selected = true;
        selectedObject = curr;
    }
    private Vector2 GetMousePosition()
    {
        return m_Camera.ScreenToWorldPoint(Input.mousePosition);
    }
    private GameObject GetObjectUnderMouse()
    {
        var origin = GetMousePosition();//czy nie vector3??[!!!!]
        var hit = Physics2D.Raycast(origin, Vector2.zero);
        if (hit.collider != null) return hit.collider.gameObject;//return hit.collider != null ? hit.collider.gameObject : null;

        //(A)
        return EventSystem.current.currentSelectedGameObject;

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
}
