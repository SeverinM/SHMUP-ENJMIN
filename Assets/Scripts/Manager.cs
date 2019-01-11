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
    Level lvl;

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
        lvl.OnNextLevel += Lvl_OnNextLevel;
    }

    /// <summary>
    /// Passage a la prochaine parcelle de niveau 
    /// </summary>
    /// <param name="nextLevel"></param>
    private void Lvl_OnNextLevel(Level nextLevel)
    {
        AddToStack(nextLevel);
        nextLevel.OnNextLevel += Lvl_OnNextLevel;
    }

    // Update is called once per frame
    void Update () {
        foreach (BaseInput bI in allInputs)
        {
            bI.UpdateInput();
        }
    }

    /// <summary>
    /// On ajoute une couche au dessus du layer
    /// </summary>
    /// <param name="lay"></param>
    public void AddToStack(Layers lay)
    {
        if( allLayers.Count > 0)
            allLayers.Peek().OnFocusLost();

        allLayers.Push(lay);
        allLayers.Peek().Init(allInputs);
        allLayers.Peek().OnFocusGet();
    }

    /// <summary>
    /// On supprime la derniere couche , cela detruit le gameobject
    /// </summary>
    public void PopToStack()
    {
        allLayers.Peek().OnFocusLost();
        Destroy(allLayers.Pop().gameObject);
        allLayers.Peek().OnFocusGet();
    }
}
