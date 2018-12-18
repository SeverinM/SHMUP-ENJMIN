using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe singleton et indestructibles ayant pour but de tenir des references d'autres objets et de gerer les layers
/// </summary>
public class Manager : MonoBehaviour {

    static Manager instance;
    List<BaseInput> allInputs = new List<BaseInput>();
    Stack<Layers> allLayers = new Stack<Layers>();

    [SerializeField]
    Layers lvl;

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
        allInputs.Add(new KeyBoardInput());
        allInputs.Add(new MouseInput());
        allInputs.Add(new ControllerInput());
        

        AddToStack(lvl);
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
        allLayers.Push(lay);
        allLayers.Peek().Init(allInputs);
        allLayers.Peek().OnFocusGet();
    }

    public void PopToStack()
    {
        allLayers.Peek().OnFocusLost();
        Destroy(allLayers.Pop().gameObject);
        allLayers.Peek().OnFocusGet();
    }
}
