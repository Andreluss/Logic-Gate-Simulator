using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class BaseRenderer : MonoBehaviour
{
    protected BaseRenderer()
    {
    }

    public virtual void Draw() { }
    public virtual bool Selected { 
        set
        {
            if(value) EnableOutline();
            else DisableOutline();
        }
    }
    
    //[TODO] [GUI]
    public virtual void EnableOutline()
    {
        //throw new NotImplementedException();
    }

    //[TODO] [GUI]
    public virtual void DisableOutline()
    {
        //throw new NotImplementedException();
    }
    //[TODO] [GUI] 
    //dorysowywac outline w zaleznosci od typu obiektu
}
