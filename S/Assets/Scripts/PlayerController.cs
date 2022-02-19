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
    }

    private CollisionData selectedObject;
    private Node selectedNode;
    private void StateIdleStart()
    {
        PlayerState = State.Idle;
    }
    private void StateIdle()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var obj = GetObjectUnderMouse();
            if (obj != null) Debug.Log("clicked " + GetObjectUnderMouse().name);

            selectedObject = obj.GetComponent<CollisionData>();
            ChangeSelection(selectedObject, selectedObject);

            if (selectedObject is NodeCollision)
            {
                selectedNode = (selectedObject as NodeCollision).node;
                StateMachine.ChangeState(new StateMachine.PlayerState(StateNodeDrag));
            }
        }
    }
    private void StateIdleEnd()
    {

    }


    Vector2 mousePositionLMBDown;
    Vector2 selectedNodePositionLMBDown;
    private void StateNodeDragStart()
    {
        Debug.Log("NodeDrag STATE");
        PlayerState = State.NodeDrag;
        mousePositionLMBDown = GetMousePosition();
        Debug.Assert(selectedNode != null);
        selectedNodePositionLMBDown = selectedNode.Position;
    }
    private void StateNodeDrag()
    {
        var deltaPosition = GetMousePosition() - mousePositionLMBDown;
        if(deltaPosition.magnitude > 0.2f) //zeby przez przypadek nie przesuwaæ o 0.001mm
            selectedNode.Position = selectedNodePositionLMBDown + deltaPosition;

        if (!Input.GetMouseButton(0)) //jesli juz nie trzymamy LPM
        {
            StateMachine.ChangeState(new StateMachine.PlayerState(StateIdle));
        }
    }
    private void StateNodeDragEnd()
    {

    }



    /* Pomocnicze funkcje odsyfiaj¹ce resztê kodu: */
    private void ChangeSelection(CollisionData prev, CollisionData curr)
    {
        if(prev == curr) return;

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
