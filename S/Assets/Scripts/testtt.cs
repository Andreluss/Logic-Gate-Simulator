using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testtt : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if(hit.collider != null)
            {
                Debug.Log($"we've just hit {hit.collider.name}");
            }
        }
    }
}
