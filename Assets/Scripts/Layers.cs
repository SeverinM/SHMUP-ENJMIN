using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Layers : MonoBehaviour {

    protected List<BaseInput> refInput = new List<BaseInput>();

    public void Init(List<BaseInput> refs)
    {
        refInput = refs;
    }

    public abstract void OnFocusGet();
    public abstract void OnFocusLost();
}
