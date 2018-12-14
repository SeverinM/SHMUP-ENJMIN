using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : Layers {

    [SerializeField]
    UnityEngine.UI.Text test;

    Binding<int> test2;
    public override void OnFocusGet()
    {        
    }

    public override void OnFocusLost()
    {
    }

    public void Start()
    {
        test2 = new Binding<int>(test);
        test2.WatchedValue = 9;
    }
}
