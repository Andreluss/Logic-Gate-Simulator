//TODO: ogarnac jakos te 2,4,8bit. inputy

public class InputNode : Node
{
    public InputNode(bool hidden=false) : base(0, 1, "Input", hidden)
    {
    }

    //Overrides
    public override int GetTemplateID()
    {
        return 0;
    }

    //Special functions of this type of node
    public void SetValue(bool value)
    {
        outVals[0] = value;
    }
    public void FlipValue()
    {
        outVals[0] = !outVals[0];
    }

    public bool GetValue()
    {
        return outVals[0];
    }
}
