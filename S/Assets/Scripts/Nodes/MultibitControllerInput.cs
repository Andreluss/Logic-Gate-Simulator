using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MultibitControllerInput : MultibitController
{
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
    public MultibitControllerInput(int bitCount, bool hidden) : base(bitCount, hidden)
    {
        //...
        //dopasuj rozmiar calego kloca
        Inputs = new List<InputNode>();
        for (int i = 0; i < bitCount; i++)
        {
            //create new InputNode
            Vector2 pos = new(0, -bitCount + i); // <== [TODO] wyliczyc to
            Inputs[i] = NodeManager.CreateNode(AppSaveData.InputTemplate, pos) as InputNode;
            Inputs[i].Controller = this;
        }
    }

    public MultibitControllerInput(List<InputNode> forInputs, bool hidden) : base(forInputs.Count, hidden)
    {
        Inputs = forInputs;
        throw new NotImplementedException();
    }
    
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
    private MBIRenderer renderer;
    private int value;

    public override NodeRenderer GetRenderer()
    {
        return renderer;
    }
    protected override void CreateRenderer()
    {
        throw new NotImplementedException();
    }

    protected override void DestroyRenderer()
    {
        throw new NotImplementedException();
    }

    public override int GetTemplateID()
    {
        return BitCount switch
        {
            2 => 2,
            4 => 3,
            8 => 4,
            _ => throw new Exception("This multibit input controller doesn;t have a template ID"),
        };
    }
}
