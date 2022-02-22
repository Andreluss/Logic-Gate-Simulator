using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(StateMachine))]
public class PlayerController : MonoBehaviour
{
    private Camera m_Camera;
    private StateMachine StateMachine;
    

    private void Awake()
    {
        m_Camera = Camera.main; 
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

    /* Klikniêcia guzików */
    public void OnSaveAsTemplateClick()
    {
        StateMachine.ChangeState(new StateMachine.PlayerState(StateNodeNew));
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

    private int newNodeTemplateID;
    //Node selectedNode;
    //bool startedDragging;





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

        if(!Input.GetMouseButton(0))
        {
            edgeNewRend.Destroy();
            if (currentInputSocket != null)
            {
                if(edgeNewRend.from != currentInputSocket.targetNode)
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
    }
    private void StateNodeInteract()
    {
        var deltaPosition = GetMousePosition() - mousePositionLMBDown;
        if (startedDragging || deltaPosition.magnitude > 0.2f) //zeby przez przypadek 
        {                                                       //nie przesuwaæ o 0.001mm
            startedDragging = true; //Debug.Log("draggin"); 
            Vector2 newPos = selectedNodePositionLMBDown + deltaPosition;
            if (AppSaveData.Settings.SnapObjects != Input.GetKey(KeyCode.LeftControl))//albo albo
            {
                //wtedy snapujemy do siatki
                float dist = AppSaveData.Settings.SnapDistance;
                newPos.x = Mathf.Round(newPos.x / dist) * dist;
                newPos.y = Mathf.Round(newPos.y / dist) * dist;
                Debug.Log(newPos);
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
    private void ChangeSelectionTo(CollisionData curr)
    {
        var prev = selectedObject;
        if(prev == curr) return;
        Debug.Log($"Selected obj is now {curr.name}");
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
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
