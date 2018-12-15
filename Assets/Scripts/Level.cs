using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : Layers {

    [SerializeField]
    protected Player player;

    [SerializeField]
    GameObject prefabEnnemy;

    public override void OnFocusGet()
    {      
        foreach(BaseInput inp in refInput)
        {
            inp.OnInputExecuted += player.InterpretInput;
        }

        GameObject gob = Instantiate(prefabEnnemy);
        gob.transform.position = new Vector3(gob.transform.position.x, player.transform.position.y, gob.transform.position.z);
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
