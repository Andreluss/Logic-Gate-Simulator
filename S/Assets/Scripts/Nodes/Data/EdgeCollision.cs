using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeCollision : CollisionData
{
    public Node from;
    public int outIdx;
    public Node to;
    public int inIdx;
    private EdgeRenderer edgeRenderer;
    private PolygonCollider2D polygonCollider;
    public EdgeRenderer EdgeRenderer { get => edgeRenderer; set => edgeRenderer = value; }
    public override BaseRenderer Renderer { get => EdgeRenderer; }
    public PolygonCollider2D PolygonCollider { 
        get
        {
            if(polygonCollider == null) 
                polygonCollider = GetComponent<PolygonCollider2D>();
            return polygonCollider;
        }
        set => polygonCollider = value; 
    }

    private void Start()
    {
        
    }

    public void Initialize(Node from, int outIdx, Node to, int inIdx, EdgeRenderer edgeRenderer)
    {
        this.from = from;
        this.outIdx = outIdx;
        this.to = to;
        this.inIdx = inIdx;
        this.EdgeRenderer = edgeRenderer;
        //[TODO] [MATH]
        //edit collider polygons

        RecalcCollider();
    }

    public void Initialize(Node from, int outIdx, Vector2 endpoint, EdgeRenderer edgeRenderer)
    {
        this.from = from;
        this.outIdx = outIdx;
        this.EdgeRenderer = edgeRenderer;

        RecalcCollider();
    }

    void RecalcCollider()
    {
        var lr = EdgeRenderer.GetComponent<LineRenderer>();

        var n = lr.positionCount;
        var positions = new Vector3[n];
        lr.GetPositions(positions);

        Debug.Log(positions);

        float grubosc = 0.1f;
        //tutaj start
        PolygonCollider.pathCount = positions.Length - 1;
        //dla kazdej krawedzi prostok¹t
        for(int i = 0; i+1 < n; i++)
        {
            var p = positions[i];
            var q = positions[i+1];

            List<Vector2> path = new();
            //wsm to wystarczy tutaj wyliczyc te 4 punkty
            //....
            
            
            
            path.Add(new Vector2(p.x * 0.8f, p.y + 3));//na przyklad cos takiego
            path.Add(new Vector2(p.x + 1.1f, p.y - 0.2f));



            //....

            PolygonCollider.SetPath(i, path);
        }
    }
}
