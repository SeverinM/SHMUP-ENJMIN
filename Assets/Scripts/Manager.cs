using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour {

    static Manager instance;
    List<BaseInput> allInputs = new List<BaseInput>();
    Stack<Layers> allLayers;

	// Use this for initialization
	void Start () {
		if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
	}
	
	// Update is called once per frame
	void Update () {
		foreach (BaseInput bI in allInputs)
        {
            bI.UpdateInput();
        }
	}

    public void AddToStack(Layers lay)
    {
        allLayers.Peek()?.OnFocusLost();
        allLayers.Push(lay);
        allLayers.Peek().Init(allInputs);
        allLayers.Peek().OnFocusGet();
    }

    public void PopToStack()
    {
        allLayers.Peek()?.OnFocusLost();
        Destroy(allLayers.Pop().gameObject);
        allLayers.Peek()?.OnFocusGet();
    }
}
