using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

/// <summary>
/// Classe singleton et indestructibles ayant pour but de tenir des references d'autres objets et de gerer les layers
/// </summary>
public class Manager : MonoBehaviour {

    static Manager instance;
    List<BaseInput> allInputs = new List<BaseInput>();
    Stack<Layers> allLayers = new Stack<Layers>();

    [HideInInspector]
    public int Count = 0;


    [SerializeField]
    Layers firstLayer;

    [SerializeField]
    Vector3 cameraPositionRelative;
    public Vector3 CameraPositionRelative
    {
        get
        {
            return cameraPositionRelative;
        }
    }

    int countLayer = 0;
    public int CountLayer
    {
        get
        {
            return countLayer;
        }
    }

    Player player;
    public Player Player
    {
        get
        {
            if (TopLayer is Level)
            {
                return ((Level)TopLayer).Player;
            }
            return null;
        }
    }

    [SerializeField]
    int baseLife = 3;
    public int BaseLife
    {
        get
        {
            return baseLife;
        }
    }

    public Layers TopLayer
    {
        get
        {
            if (allLayers.Count > 0)
            {
                return allLayers.Peek();
            }
            return null;
        }
    }
    
    private ServerConnection connection = new ServerConnection();

    public void IncreaseCount()
    {
        countLayer++;
    }

    public void ResetCount()
    {
        countLayer = 0;
    }

    // Use this for initialization
    void Awake()
    {
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

        DontDestroyOnLoad(instance);
    }

    public void ResetLife()
    {
        if (Player != null)
            Player.Life = BaseLife;
    }

    private void Start()
    {
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        AddToStack(firstLayer);
    }

    //Lorsqu'une scene est chargé chercher le IsFirst
    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        Layers lay = GameObject.FindObjectsOfType<Layers>().Where(x => x.IsFirst).First();
        lay = FindSubsequentLayer(countLayer, lay);
        AddToStack(lay);
    }

    public void PopAll()
    {
        while(allLayers.Count > 0)
        {
            PopToStack();
        }
    }

    public void EnableMenu(Menu menu)
    {
        Constants.SetAllConstants(0);
        AddToStack(menu);
    }

    public static Manager GetInstance()
    {
        if (instance == null)
        {
            instance = new GameObject().AddComponent<Manager>();
            DontDestroyOnLoad(instance);
        }
        return instance;
    }

    /// <summary>
    /// Passage a la prochaine parcelle de niveau 
    /// </summary>
    /// <param name="nextLevel"></param>
    private void Lvl_OnNextLevel(Level nextLevel)
    {
        AddToStack(nextLevel);
        StartCoroutine(connection.PostScores(Constants.PlayerName, Constants.TotalScore));
        nextLevel.OnNextLevel += Lvl_OnNextLevel;
        countLayer++;
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
    public void AddToStack(Layers lay, bool destroyPrevious = false)
    {
        Count++;
        if(allLayers.Count > 0)
        {
            if (allLayers.Peek() != null)
            {
                allLayers.Peek().OnFocusLost();
                if (destroyPrevious)
                {
                    Destroy(allLayers.Pop().gameObject);
                }
            }
            else
            {
                //Dans le cas où le dessus du tas est un null
                if (destroyPrevious)
                {
                    allLayers.Pop();
                }
            }
        }
        allLayers.Push(lay);
        allLayers.Peek().Init(allInputs);
        allLayers.Peek().OnFocusGet();
        if (allLayers.Peek() is Level)
        {
            ((Level)allLayers.Peek()).OnNextLevel += Lvl_OnNextLevel;
        }
    }

    /// <summary>
    /// On supprime la derniere couche , cela detruit le gameobject
    /// </summary>
    public void PopToStack()
    {
        if (allLayers.Peek() != null)
        {
            allLayers.Peek().OnFocusLost();
        }

        allLayers.Pop();
        
        if (allLayers.Count > 0 && allLayers.Peek() != null)
            allLayers.Peek().OnFocusGet();
    }

    public static Layers FindSubsequentLayer(int depth,Layers firstLayer)
    {
        Layers output = firstLayer;
        int count = 0;
        while (depth > count)
        {
            if (output.NextLevel != null)
            {
                output = output.NextLevel;
            }
            count++;
        }
        return output;
    }
}
