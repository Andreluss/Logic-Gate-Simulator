using UnityEngine;
public class InputNode : Node
{
    public InputNode(bool hidden=true) : base(0, 1, "Input", hidden)
    {
    }
    public InputRenderer renderer;
    //Overrides
    public override int GetTemplateID()
    {
        return 0;
    }
    protected override void CreateRenderer()
    {
        Debug.Log("creating innode rend");
        renderer = InputRenderer.Make(this);
    }
    protected override void DestroyRenderer()
    {
        Object.Destroy(renderer.transform.parent);//destroy even the root
    }
    public override BaseRenderer GetRenderer()
    {
        return renderer;
    }

    //Special functions of this type of node
    public void SetValue(bool value)
    {
        outVals[0] = value;
        if(renderer != null)
            renderer.HandleValue(value);
    }
    public void FlipValue()
    {
        outVals[0] = !outVals[0];
        if (renderer)
            renderer.HandleValue(outVals[0]);
    }
    public bool GetValue()
    {
        return outVals[0];
    }
}
