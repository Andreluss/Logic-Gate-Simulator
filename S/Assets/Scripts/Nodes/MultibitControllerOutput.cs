using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultibitControllerOutput : MultibitController //wlasciwie taki meta node
{
    /* ----- constr ----- */
    public MultibitControllerOutput(int bitCount, bool hidden) : base(bitCount, true)
    {
        Hidden = hidden;
        //...
        //dopasuj rozmiar calego kloca
        Outputs = new List<OutputNode>();
        for (int i = 0; i < bitCount; i++)
        {
            Outputs.Add(NodeManager.CreateNode(AppSaveData.OutputTemplate, Vector2.zero) as OutputNode);
            Outputs[i].GetRenderer().transform.parent.SetParent(GetRenderer().transform.parent, false);
            Outputs[i].Controller = this;
            Outputs[i].Description = "bit " + i;
        }

    }

    public MultibitControllerOutput(List<OutputNode> forOutputs, bool isHidden) : base(forOutputs.Count, true)
    {
        Outputs = forOutputs;
        Hidden = isHidden;

        for (int i = 0; i < BitCount; i++)
        {
            Outputs[i].GetRenderer().transform.parent.SetParent(GetRenderer().transform.parent, false);
            Outputs[i].Controller = this;
            Outputs[i].Description = "bit " + i;
        }
    }


    /* ----- props ----- */
    public int Value
    {
        get => value;
        private set
        {
            this.value = value;
            if (renderer != null)
                renderer.HandleValue(value);
        }
    }
    public int MaxValue { get => (1 << (BitCount - Convert.ToInt32(Signed))) - 1; }
    public int MinValue { get => Signed ? -(1 << BitCount) : 0; }
    public bool Signed { get; set; }//[TODO] ogarn¹æ guzik, który to zmienia
    public List<OutputNode> Outputs { get; }


    /* ----- overrides ----- */
    public override void Calculate()
    {
        // wypadaloby wywolywac ta funkcjê dopiero po obliczeniu wszystkich normalnych node'ow
        int newValue = 0;
        for (int i = 0; i < BitCount - 1; i++)
        {
            if (Outputs[i].GetValue())
                newValue += 1 << i;
        }
        if (Outputs[BitCount - 1].GetValue())
        {
            if (Signed)
                newValue -= 1 << (BitCount - 1);
            else
                newValue += 1 << (BitCount - 1);
        }
        Value = newValue;
    }

    public override NodeRenderer GetRenderer()
    {
        return renderer;
    }

    protected override void CreateRenderer()
    {
        renderer = MBORenderer.Make(this);
        //Calculate();
    }

    protected override void DestroyRenderer()
    {
        foreach (var node in Outputs)
        {
            NodeManager.DeleteNode(node);
        }
        UnityEngine.Object.Destroy(renderer.transform.parent.gameObject);
    }

    public override int GetTemplateID()
    {
        return BitCount switch
        {
            2 => 5,
            4 => 6,
            8 => 7,
            _ => throw new Exception("This multibit output controller doesn;t have a template ID"),
        };
    }


    /* ----- private stuff -----  */
    private MBORenderer renderer;
    private int value;
}
