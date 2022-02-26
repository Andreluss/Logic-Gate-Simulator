using System.Collections.Generic;
using System;
using UnityEngine;

public class MultibitControllerInput : MultibitController
{

    /* ----- constr ----- */
    public MultibitControllerInput(int bitCount, bool hidden) : base(bitCount, true)
    {
        Hidden = hidden;
        //...
        //dopasuj rozmiar calego kloca
        Inputs = new List<InputNode>();
        for (int i = 0; i < bitCount; i++)
        {
            //Vector2 pos = new(0, -bitCount + i); // <== [TODO] wyliczyc to
            Inputs.Add(NodeManager.CreateNode(AppSaveData.InputTemplate, Vector2.zero) as InputNode);
            Inputs[i].GetRenderer().transform.parent.SetParent(GetRenderer().transform.parent, false);
            Inputs[i].Controller = this;
        }
    }

    public MultibitControllerInput(List<InputNode> forInputs, bool hidden) : base(forInputs.Count, true)
    {
        Inputs = forInputs;
        Hidden = hidden;//bo inaczej renderer nie widzi inputow!!!
        //chyba tyle [TODO] check czy faktycznie tyle
    }


    /* ----- props ----- */
    public int Value
    {
        get => value;
        set {
            this.value = value;
            if(renderer != null)
                renderer.HandleValue(value);
        }
    }
    public int MaxValue { get => (1 << (BitCount - Convert.ToInt32(Signed))) - 1; }
    public int MinValue { get => Signed ? -(1 << BitCount) : 0; }
    public bool Signed { get; set; }//[TODO] ogarn¹æ guzik, który to zmienia
    public List<InputNode> Inputs { get; private set; }
    

    /* ----- overrides ----- */
    public override void Calculate()
    {
        //[TODO] update Value based on inputs
        // ale wypadaloby wywolywac ta funkcjê dopiero po obliczeniu wszystkich normalnych node'ow
        int newValue = 0;
        for (int i = 0; i < BitCount-1; i++)
        {
            if (Inputs[i].GetValue())
                newValue += 1 << i;
        }
        if (Signed)
            newValue -= 1 << (BitCount - 1);
        else
            newValue += 1 << (BitCount - 1);
        Value = newValue;
    }
    public override NodeRenderer GetRenderer()
    {
        return renderer;
    }
    protected override void CreateRenderer()
    {
        renderer = MBIRenderer.Make(this);
    }

    protected override void DestroyRenderer()
    {
        UnityEngine.Object.Destroy(renderer.transform.parent.gameObject);
        Debug.Log($"MBI renderer {renderer} destroyed");
    }

    public override int GetTemplateID()
    {
        return BitCount switch
        {
            2 => 1,
            4 => 2,
            8 => 3,
            _ => throw new Exception("This multibit input controller doesn;t have a template ID"),
        };
    }


    /* ----- private stuff -----  */
    private MBIRenderer renderer;
    private int value;

}
