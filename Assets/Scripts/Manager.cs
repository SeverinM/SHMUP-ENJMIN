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
                if (((Level)output) != null)
                {
                    Destroy(((Level)output).BackgroundToHide);
                }
                output = output.NextLevel;
            }
            count++;
        }
        return output;
    }


    // ScreenShake
    float DefaultShakeAmount = 0.5f;
    float ShakeAmount; // Montant de secousse
    float DefaultShakeDuration = 0.011f;
    float ShakeDuration = 0.2f; // La duration de la secousse
    float ShakePercentage = 0.2f; // Le pourcentage de 0 à 1 représentant le montant de secousse appliquée

    float startAmount; // Le montant au départ de la secousse
    float startDuration; // La durée de secousse

    bool isRunning = false; // La coroutine est en route ?

    bool smooth;
    float smoothAmount = 5f; // Montant d'adouci

    public void ShakeCamera(float amount, float duration)
    {
        ShakeAmount = amount;
        ShakeDuration = duration;
        startAmount = ShakeAmount; // Remettre par défault pour determiner un pourcentage
        startDuration = ShakeDuration; // Remettre par défault le temps de départ

        if (!isRunning) StartCoroutine(Shake());  // Appeler la corroutine que si elle n'est pas déja en cours d'execution
    }

    private IEnumerator Shake()
    {
        isRunning = true;

        while (ShakeDuration > 0.01f)
        {
            Vector3 rotationAmount = UnityEngine.Random.insideUnitSphere * ShakeAmount; // Montant de rotation à ajouter à la rotation locale
            rotationAmount.x = 90;

            ShakePercentage = ShakeDuration / startDuration; // Utilisé pour définir le montant de secousse (% * startAmount)

            ShakeAmount = startAmount * ShakePercentage; // Définir le montant de secousse (% * startAmount)
            ShakeDuration = Mathf.Lerp(ShakeDuration, 0, Time.deltaTime); // Lerp le temps pour moins de secousses vers la fin

            Camera.main.transform.localRotation = Quaternion.Euler(rotationAmount); //Le montant de rotation devient la rotation Locale

            yield return null;
        }
        Camera.main.transform.localRotation = Quaternion.AngleAxis(90, Vector3.right); // Rotation locale à 0 pour eviter que cela continue de secouer
        isRunning = false;
    }
}
