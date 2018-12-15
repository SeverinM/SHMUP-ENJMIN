using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : Layers {

    [SerializeField]
    protected Player player;

    public override void OnFocusGet()
    {      
        foreach(BaseInput inp in refInput)
        {
            inp.OnInputExecuted += player.InterpretInput;
        }
    }

    public override void OnFocusLost()
    {
        foreach (BaseInput inp in refInput)
        {
            inp.OnInputExecuted -= player.InterpretInput;
        }
    }

    public void Start()
    {

    }
}
