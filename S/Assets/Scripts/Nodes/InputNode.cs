using UnityEngine;
public class InputNode : Node
{
    public InputNode(bool hidden = true) : base(0, 1, "Input", hidden)
    {
    }
    private InputRenderer renderer;


    private MultibitControllerInput controller;
    public MultibitControllerInput Controller
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
        Object.Destroy(renderer.transform.parent.gameObject);//destroy even the root
    }
    public override NodeRenderer GetRenderer()
    {
        return renderer;
    }
    public override string Description { get => base.Description; set
        {
            base.Description = value;
            var rend = GetRenderer() as InputRenderer;
            if (rend != null)
                rend.UpdateDescription();
        }
    }

    //Special functions of this type of node
    public void SetValue(bool value)
    {
        outVals[0] = value;
        if (renderer != null)
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
