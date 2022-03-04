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
    private LineRenderer LR;
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

    private void Awake()
    {
        LR = GetComponent<LineRenderer>();
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

    public void RecalcCollider()
    {
        var lr = EdgeRenderer.GetComponent<LineRenderer>();

        var n = lr.positionCount;
        var positions = new Vector3[n];
        lr.GetPositions(positions);

        Debug.Log(positions);

        //float grubosc = 0.1f;
        //tutaj start
        PolygonCollider.pathCount = positions.Length - 1;
        //dla kazdej krawedzi prostok¹t
        for(int i = 0; i+1 < n; i++)
        {
            var p = positions[i];
            var q = positions[i+1];

            List<Vector2> path = CalculateColliderPoints(p, q);
            //ciebiera dziadu miales to zrobic
            PolygonCollider.SetPath(i, path);
        }
    }
    private List<Vector2> CalculateColliderPoints(Vector2 p, Vector2 q)
    {
        //Get the Width of the Line
        float widthDelta = LR.startWidth / 2;

        var dir = (Vector2)(q - p).normalized;
        var left = dir.Rot90CCW();
        var right = dir.Rot90CW();

        List<Vector2> colliderPositions = new List<Vector2>
        {
            p + left * widthDelta, 
            p + right * widthDelta,
            q + right * widthDelta,
            q + left * widthDelta
        };

        return colliderPositions;

        //gowno to nie dziala
        ////m = (y2 - y1) / (x2 - x1)
        //float m = (positions[1].y - positions[0].y) / (positions[1].x - positions[0].x);
        //float deltaX = (width / 2f) * (m / Mathf.Pow(m * m + 1, 0.5f));
        //float deltaY = (width / 2f) * (1 / Mathf.Pow(1 + m * m, 0.5f));

        ////Calculate the Offset from each point to the collision vertex
        //Vector3[] offsets = new Vector3[2];
        //offsets[0] = new Vector3(-deltaX, deltaY);
        //offsets[1] = new Vector3(deltaX, -deltaY);

        ////Generate the Colliders Vertices
        //List<Vector2> colliderPositions = new List<Vector2> {
        //    positions[0] + offsets[0],
        //    positions[1] + offsets[0],
        //    positions[1] + offsets[1],
        //    positions[0] + offsets[1]
        //};

        //return colliderPositions;
    }
}
