using UnityEngine;
public class OutputNode : Node
{
    public OutputNode(bool hidden) : base(1, 0, "Output", hidden)
    {
    }
    public OutputRenderer renderer;
    //Overrides
    public override void Calculate()
    {
        if (renderer != null)
            renderer.HandleValue(inVals[0]);
    }
    public override int GetTemplateID()
    {
        return 1;
    }
    protected override void CreateRenderer()
    {
        Debug.Log("creating outnode rend");
        renderer = OutputRenderer.Make(this);
    }
    protected override void DestroyRenderer()
    {
        Object.Destroy(renderer.transform.parent.gameObject);
    }
    public override NodeRenderer GetRenderer()
    {
        return renderer;
    }

    public bool GetValue() => inVals[0];
}
