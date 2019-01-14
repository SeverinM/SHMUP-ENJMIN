using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Cette classe contient un groupe de gameobject et peut etre vu comme un ensemble de "couche" , il est possible de configurer le comportement selon si la couche est en premier plan ou non
/// </summary>
public abstract class Layers : MonoBehaviour {

    [SerializeField]
    bool isFirst;
    public bool IsFirst
    {
        get
        {
            return isFirst;
        }
    }

    protected List<BaseInput> refInput = new List<BaseInput>();

    public void Init(List<BaseInput> refs)
    {
        refInput = refs;
    }

    public abstract void OnFocusGet();
    public abstract void OnFocusLost();
}
