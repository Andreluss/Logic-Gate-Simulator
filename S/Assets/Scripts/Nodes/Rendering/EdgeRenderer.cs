using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeRenderer : BaseRenderer
{
    public static EdgeRenderer Make(Node A, int outIdx, Node B, int inIdx)
    {
        var edgeGO = Object.Instantiate(
            Resources.Load<GameObject>("Sprites/Edge"));
        var edgeRend = edgeGO.GetComponent<EdgeRenderer>();
        edgeRend.C = edgeGO.GetComponent<PolygonCollider2D>();
        edgeRend.LR = edgeGO.GetComponent<LineRenderer>();
        edgeRend.start = A.GetRenderer().outSocketRends[outIdx].transform.position;
        edgeRend.end = B.GetRenderer().inSocketRends[inIdx].transform.position;
        edgeRend.UpdateRenderer();

        var edgeColl = edgeGO.GetComponent<EdgeCollision>();
        edgeColl.Initialize(A, outIdx, B, inIdx, edgeRend);

        return edgeRend;
    }
    private void SetProperZIndex() { start.z = end.z = 1; } // na chama niech bedzie
    private Vector3 start;
    private Vector3 end;
    private PolygonCollider2D C;
    private LineRenderer LR;

    private void UpdateRenderer()
    {
        SetProperZIndex();
        //buhahaha
        var positions = new List<Vector3>();
        positions.Add(start);
        if(Mathf.Abs(start.y - end.y) > 0.1f)
        {
            float dx = end.x - start.x;
            positions.Add(new Vector3(start.x + dx/2, start.y, C.transform.position.z));
            positions.Add(new Vector3(start.x + dx/2, end.y, C.transform.position.z));
        }
        positions.Add(end);
        LR.positionCount = positions.Count;
        LR.SetPositions(positions.ToArray());
    }

    private void UpdateCollider()
    {
        // [TODO]
    }

    public bool Value { get; set; }
    public Vector2 Start
    {
        get => start; 
        set
        {
            start = value;
            UpdateRenderer();
            UpdateCollider();
        }
    }
    public Vector2 End
    {
        get => end; 
        set
        {
            end = value;
            UpdateRenderer();
            UpdateCollider();
        }
    }
}
