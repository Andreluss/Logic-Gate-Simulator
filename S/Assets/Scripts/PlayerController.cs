using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Camera m_Camera;
    StateMachine StateMachine;
    // Start is called before the first frame update
    void Start()
    {
        m_Camera = Camera.main;
    }

    private GameObject GetObjectUnderMouse()
    {
        var origin = m_Camera.ScreenToWorldPoint(Input.mousePosition);
        var hit = Physics2D.Raycast(origin, Vector2.zero);
        if (hit.collider != null) 
            Debug.Log(hit.collider.gameObject);
        return hit.collider.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Debug.Log("clicked " + GetObjectUnderMouse().name);
        }
    }

    
}
