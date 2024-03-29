using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeRenderer : BaseRenderer
{
    public Material materialON, materialOFF;
    public Node from;
    public int outIdx;
    public Node to;
    public int inIdx;
    public static EdgeRenderer Make(Node A, int outIdx, Vector2 endpoint)
    {
        //troche copy-pasta ale 
        var edgeGO = Object.Instantiate(
            Resources.Load<GameObject>("Sprites/Edge"));
        var edgeRend = edgeGO.GetComponent<EdgeRenderer>();

        edgeRend.from = A;
        edgeRend.outIdx = outIdx;

        //edgeRend.C = edgeGO.GetComponent<PolygonCollider2D>();
        edgeRend.LR = edgeGO.GetComponent<LineRenderer>();
        edgeRend.start = A.GetRenderer().outSocketRends[outIdx].transform.position;
        //edgeRend.end = B.GetRenderer().inSocketRends[inIdx].transform.position;
        edgeRend.end = endpoint;

        edgeRend.edgeColl = edgeGO.GetComponent<EdgeCollision>();
        edgeRend.edgeColl.Initialize(A, outIdx, endpoint, edgeRend);

        edgeRend.UpdateRenderer();
        edgeRend.HandleState(A.outVals[outIdx]);//[BUG?] 
        return edgeRend;
    }
    public static EdgeRenderer Make(Node A, int outIdx, Node B, int inIdx)
    {
        var edgeGO = Object.Instantiate(
            Resources.Load<GameObject>("Sprites/Edge"));
        var edgeRend = edgeGO.GetComponent<EdgeRenderer>();
        
        edgeRend.from = A;
        edgeRend.outIdx = outIdx;
        edgeRend.to = B;
        edgeRend.inIdx = inIdx;

        //edgeRend.C = edgeGO.GetComponent<PolygonCollider2D>();
        edgeRend.LR = edgeGO.GetComponent<LineRenderer>();
        edgeRend.start = A.GetRenderer().outSocketRends[outIdx].transform.position;
        edgeRend.end = B.GetRenderer().inSocketRends[inIdx].transform.position;

        edgeRend.edgeColl = edgeGO.GetComponent<EdgeCollision>();
        edgeRend.edgeColl.Initialize(A, outIdx, B, inIdx, edgeRend);

        edgeRend.UpdateRenderer();
        return edgeRend;
    }
    private void SetProperZIndex() { start.z = end.z = 1; } // na chama niech bedzie
    private Vector3 start;
    private Vector3 end;
    private LineRenderer LR;
    private EdgeCollision edgeColl;

    /* bardzo pomocnicze? funkcje */
    private float GetBorderY(Node node, float dir)
    {
        Transform obj = from.GetRenderer().transform;
        return obj.position.y + obj.localScale.y / 2 * dir;
    }

    private void UpdateRenderer()
    {
        SetProperZIndex();
        var positions = new List<Vector3>();
        positions.Add(start);

       
        if(start.x > end.x + 0.1f)
        {
            float offsetX = 0.4f, offsetY = 0.4f;
            
            
            positions.Add(start + Vector3.right * offsetX);


            //if (start.y > end.y)
            //{
            //    float top_y = obj.position.y + obj.localScale.y / 2;
            //    ypos = top_y;
            //}
            //else
            //{
            //    float bottom_y = obj.position.y - obj.localScale.y / 2;
            //    ypos = bottom_y;
            //}

            //float ypos = 0;
            //if(start.y < end.y)
            //{
            //    float topA = GetBorderY(from, 1f),
            //          bottomA = GetBorderY(from, -1f);
            //    if(to != null)
            //    {
            //        float topB = GetBorderY(to, 1f),
            //              bottomB = GetBorderY(to, -1f);

            //        if(topA < bottomB)
            //        {
            //            //git
            //            ypos = (topA + bottomB) / 2;
            //        }
            //        else
            //        {
            //            ypos = topB + offsetY;
            //        }
            //    }
            //    else
            //    {
            //        if(topA < end.y)
            //        {
            //            ypos = (topA + end.y) / 2;
            //        }
            //        else
            //        {
            //            ypos = end.y + offsetY;
            //        }
            //    }
            //}
            //else
            //{
            //    if (to != null)
            //    {

            //    }
            //    else
            //    {

            //    }
            //}

            var dir = (start.y > end.y ? 1 : -1);

            Transform obj = from.GetRenderer().transform;
            float ypos = obj.position.y + dir * (offsetY + obj.localScale.y / 2);

            if (Mathf.Abs(ypos - end.y) < offsetY)
            {
                ypos = obj.position.y + (end.y - start.y) / 2;
            }






            var next = start + Vector3.right * offsetX;
            next.y = ypos;

            positions.Add(next);


            next = end - Vector3.right * offsetX;
            next.y = ypos;

            positions.Add(next);



            positions.Add(end - Vector3.right * offsetX);

            //cos nie tak
            //Mathf.Pow(start.x, 0.5f);
            //positions.Add(new Vector3(start.x + 10 / 3, start.y + 5, C.transform.position.z));
        }
        else if (Mathf.Abs(start.y - end.y) > 0.25f)
        {
            float dx = end.x - start.x;
            positions.Add(new Vector3(start.x + dx * 2 / 3, start.y, transform.position.z));//[BuG?] by�o C.transf...
            positions.Add(new Vector3(start.x + dx * 2 / 3, end.y, transform.position.z));
        }

        positions.Add(end);


        LR.positionCount = positions.Count;
        LR.SetPositions(positions.ToArray());

        UpdateCollider();
    }

    public void UpdateCollider()
    {
        edgeColl.RecalcCollider();
    }

    public void Destroy()
    {
        Object.Destroy(this.gameObject);
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

    internal void HandleState(bool ON)
    {
        if (ON)
            LR.material = materialON;
        else
            LR.material = materialOFF;
    }
}
