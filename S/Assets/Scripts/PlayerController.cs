using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(StateMachine))]
public class PlayerController : MonoBehaviour
{
    private Camera m_Camera;
    private StateMachine StateMachine;
    

    private void Awake()
    {
        StateMachine = GetComponent<StateMachine>();
    }

    void Start()
    {
        m_Camera = Camera.main;

        StateMachine.Initialize(new StateMachine.PlayerState(StateIdle));
        PlayerState = State.Idle;
    }

    void Update()
    {
        StateMachine.UpdateStateMachine();
    }

    private void FixedUpdate()
    {

    }



    /* Stany i zmienne potrzebne do ró¿nych stanów: */

    private State PlayerState;
    public enum State
    {
        Idle,
        NodeDrag,
        EdgeNew,
        EdgeOld,
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
            if (obj != null)
            {
                Debug.Log("clicked " + GetObjectUnderMouse().name);

                selectedObject = obj.GetComponent<CollisionData>();
                ChangeSelection(selectedObject, selectedObject);

                if (selectedObject is NodeCollision)
                {
                    selectedNode = (selectedObject as NodeCollision).node;
                    StateMachine.ChangeState(new StateMachine.PlayerState(StateNodeDrag));
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
            }
        }
        if(Input.GetMouseButtonDown(1))
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
                NodeManager.Connect(edgeNewRend.from, edgeNewRend.outIdx,
                    currentInputSocket.targetNode, currentInputSocket.inIdx);
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
        }

        //[DESIGN] jesli usuniemy ten próg od³¹czenia socketu (0.2f), 
        // to wgl bêdzie mo¿na usun¹æ ca³y stan EdgeOld
        var mpos = GetMousePosition();
        if(Vector2.Distance(mpos, inSocket.transform.position) > 0.2f)
        {
            var (node, inidx) = (inSocket.targetNode, inSocket.inIdx);
            var (from, outidx) = (node.ins[inidx].st, node.ins[inidx].nd);
            
            NodeManager.Disconnect(from, outidx, node, inidx);

            edgeNewRend = EdgeRenderer.Make(from, outidx, mpos);
            StateMachine.ChangeState(new StateMachine.PlayerState(StateEdgeNew));
        }
    }
    private void StateEdgeOldEnd()
    {
    }

    InSocketCollision inSocket;





    private void StateNodeDragStart()
    {
        PlayerState = State.NodeDrag;
        mousePositionLMBDown = GetMousePosition();
        Debug.Assert(selectedNode != null);
        selectedNodePositionLMBDown = selectedNode.Position;
        startedDragging = false;
    }
    private void StateNodeDrag()
    {
        var deltaPosition = GetMousePosition() - mousePositionLMBDown;
        if (startedDragging || deltaPosition.magnitude > 0.2f) //zeby przez przypadek 
        {                                                       //nie przesuwaæ o 0.001mm
            Debug.Log("draggin");
            selectedNode.Position = selectedNodePositionLMBDown + deltaPosition;
            startedDragging = true;
        }

        if (!Input.GetMouseButton(0)) //jesli juz nie trzymamy LPM
        {
            StateMachine.ChangeState(new StateMachine.PlayerState(StateIdle));
        }
    }
    private void StateNodeDragEnd()
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
    private void ChangeSelection(CollisionData prev, CollisionData curr)
    {
        if(prev == curr) return;
        Debug.Log($"Selected obj is now {curr}");
        if (prev != null) prev.Renderer.Selected = false;
        if (curr != null) curr.Renderer.Selected = true;
    }
    private Vector2 GetMousePosition()
    {
        return m_Camera.ScreenToWorldPoint(Input.mousePosition);
    }
    private GameObject GetObjectUnderMouse()
    {
        var origin = GetMousePosition();//czy nie vector3??[!!!!]
        var hit = Physics2D.Raycast(origin, Vector2.zero);
        return hit.collider != null ? hit.collider.gameObject : null;
    }
}
