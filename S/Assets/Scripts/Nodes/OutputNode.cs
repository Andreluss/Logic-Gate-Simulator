using UnityEngine;
public class OutputNode : Node
{
    public OutputNode(bool hidden) : base(1, 0, "Output", hidden)
    {
    }
    public OutputRenderer renderer;

    private MultibitControllerOutput controller;
    public MultibitControllerOutput Controller
    {
        get => controller;
        set
        {
            controller = value;
            Controlled = true;
        }
    }

    public bool Controlled { get; private set; }

    //Overrides
    public override void Calculate()
    {
        if (renderer != null)
            renderer.HandleValue(inVals[0]);
        base.Calculate();
    }
    public override int GetTemplateID()
    {
        return 4;
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

    public override string Description
    {
        get => base.Description; set
        {
            base.Description = value;
            var rend = GetRenderer() as OutputRenderer;
            if(rend != null) 
                rend.UpdateDescription();
        }
    }

    public bool GetValue() => inVals[0];
}
