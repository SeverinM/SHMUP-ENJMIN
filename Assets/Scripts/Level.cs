using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public class Level : Layers
{

    [SerializeField]
    protected Player player;

    [SerializeField]
    protected GameObject JimPrefab;

    [SerializeField]
    protected GameObject MikePrefab;

    [SerializeField]
    protected GameObject BobPrefab;

    [SerializeField]
    protected GameObject MenuPrefab;

    [SerializeField]
    protected Level NextLevel;

    public GameObject Player
    {
        get
        {
            return player.gameObject;
        }
    }

    int indexSelection = 0;
    public int IndexSelection
    {
        get
        {
            return indexSelection;
        }
        set
        {
            indexSelection = Mathf.Clamp(value,0, allButtonsMenu.Count - 1);
        }
    }
    List<Button> allButtonsMenu = new List<Button>();

    [SerializeField]
    Text textUI;

    [SerializeField]
    Text Count;

    [SerializeField]
    GameObject canvas;

    [SerializeField]
    GameObject text;

    Navigation nav;

    int countGenerator = 0;

    Dictionary<GameObject, Text> characterTexts = new Dictionary<GameObject, Text>();
    Dictionary<GameObject, Vector3> StoredValue = new Dictionary<GameObject, Vector3>();

    [HideInInspector]
    public List<GameObject> characters = new List<GameObject>();

    [HideInInspector]
    public List<GameObject> charactersToRemove = new List<GameObject>();
    public Binding<int> watchNbSpawn = new Binding<int>(0, null);

    public delegate void LevelParam(Level nextLevel);
    public event LevelParam OnNextLevel;

    //Appellé quand le layer est au dessus de la stack
    public override void OnFocusGet()
    {
        foreach(Button btn in MenuPrefab.GetComponentsInChildren<Button>())
        {
            allButtonsMenu.Add(btn);
        }

        player.Destroyed += PlayerDied;

        //Mise en place des data bindings;
        player.SetOnLifeChanged((x) =>
        {
            if (textUI != null)
                textUI.text = "Nombre de vie : " + x.ToString();
        });

        watchNbSpawn.ValueChanged = (x) =>
        {
            if (Count != null)
                Count.text = "Nombre d'ennemies restant : " + x.ToString();
        };

        // Faire en sorte que tous les inputs notifient le joueur
        foreach (BaseInput inp in refInput)
        {
            inp.OnInputExecuted += player.InterpretInput;
            inp.OnInputExecuted += Inp_OnInputExecuted;
        }

        // Mettre en route tous les générateurs en leur attribuant un état
        foreach (Generator generator in transform.GetComponentsInChildren<Generator>())
        {
            countGenerator++;
            //Initialisation du generateur
            if (generator.AllWaves.Count > 0)
            {
                generator.TryPassWave();
            }
            generator.EveryoneDied += GeneratorDone;
            generator.WaveCleaned += Generator_WaveCleaned;
        }
        watchNbSpawn.WatchedValue = transform.GetComponentsInChildren<Generator>().Select(x => x.GetComponent<Generator>().EnnemiesLeftToSpawn).Sum();

        // Provisoirement
        GameObject toAddText = Instantiate(text, canvas.transform);
        characterTexts.Add(player.gameObject, toAddText.GetComponent<Text>());
    }

    private void Inp_OnInputExecuted(BaseInput.TypeAction tyAct, BaseInput.Actions acts, Vector2 values)
    {
        //On alterne le resultat de l'inputs
        if (tyAct.Equals(BaseInput.TypeAction.Down) && acts.Equals(BaseInput.Actions.Pause))
        {
            TogglePause();
        }

        if (tyAct.Equals(BaseInput.TypeAction.Down) && acts.Equals(BaseInput.Actions.AllMovement))
        {
            double angle = Utils.AngleBetween(Vector2.left, values);
            //On va vers le bas
            if (angle > 70 && angle < 110)
            {
                IndexSelection++;
            }
            if (angle > -110 && angle < -70)
            {
                IndexSelection--;
            }
            allButtonsMenu[IndexSelection].Select();
        }
    }

    public void TogglePause()
    {
        Constants.Pausing = !Constants.Pausing;
        Constants.SetAllConstants(Constants.Pausing ? 0 : 1);
        MenuPrefab.SetActive(Constants.Pausing);

        if (Constants.Pausing)
        {
            allButtonsMenu[0].Select();
        }
     
        foreach (GameObject gob in GameObject.FindGameObjectsWithTag("Bullet"))
        {
            if (Constants.Pausing)
            {
                StoredValue[gob] = gob.GetComponent<Rigidbody>().velocity;
                gob.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
            else
            {
                if (StoredValue.ContainsKey(gob))
                    gob.GetComponent<Rigidbody>().velocity = StoredValue[gob];
            }
        }
    }

    private void Generator_WaveCleaned(int nb, Generator who)
    {
        LockWaveElement elt = new LockWaveElement();
        elt.generator = who;
        elt.number = nb;
        foreach(Generator gen in transform.GetComponentsInChildren<Generator>().Where(x => x != who))
        {
            gen.RemoveLock(elt);
        }
    }

    public void Update()
    {
        // Enlever un personnage du niveau
        foreach (GameObject character in charactersToRemove)
        {
            character.GetComponent<Character>().SetState(null);
            characterTexts.Remove(character);
            characters.Remove(character);
            Destroy(character);
        }
        charactersToRemove.Clear();
        watchNbSpawn.WatchedValue = transform.GetComponentsInChildren<Generator>().Select(x => x.GetComponent<Generator>().EnnemiesLeftToSpawn).Sum();
    }

    public void LateUpdate()
    {
        foreach (KeyValuePair<GameObject, Text> value in characterTexts)
        {
            if (value.Key != null)
            {
                // Affichage de données depuis le GameObject
                if (value.Key.GetComponent<Character>().ActualState != null)
                {
                    value.Value.text = value.Key.GetComponent<Character>().ActualState.ToString();
                }

                // Position du texte au dessus d'un gameObject
                Vector3 offsetPos = new Vector3(value.Key.transform.position.x, value.Key.transform.position.y, value.Key.transform.position.z + 0.5f);

                // Calcul de la position à l'écran 
                Vector2 canvasPos;
                Vector2 screenPoint = Camera.main.WorldToScreenPoint(offsetPos);

                // Convertir la position à l'écran vers l'espace du canvas 
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), screenPoint, null, out canvasPos);

                value.Value.transform.localPosition = canvasPos;
            }
        }
    }

    /// <summary>
    /// Ajouter un Ennemi dans le niveau
    /// Cela permet au niveau de connaitre d'avoir une liste des ennemis instanciés
    /// </summary>
    /// <param name="character"></param>
    /// <param name="position"></param>
    public GameObject AddEnemy(Enemy.EnemyType type, Vector3 position)
    {
        GameObject character;
        switch (type)
        {
            case Enemy.EnemyType.BOB:
                character = BobPrefab;
                break;

            case Enemy.EnemyType.JIM:
                character = JimPrefab;
                break;

            case Enemy.EnemyType.MIKE:
                character = MikePrefab;
                break;

            default:
                character = BobPrefab;
                break;
        }

        // Instantier un ennemi
        GameObject toAdd = Instantiate(character, position, Quaternion.identity);
        characters.Add(toAdd);
        if(text != null)
        {
            GameObject toAddText = Instantiate(text, canvas.transform);
            characterTexts.Add(toAdd, toAddText.GetComponent<Text>());
        }      
        return toAdd;
    }

    private Player Level_TryReachingPlayer()
    {
        return player;
    }

    /// <summary>
    /// Retirer un personnage du niveau
    /// </summary>
    /// <param name="character">Le personnage à retirer</param>
    public void Remove(Character character)
    {
        charactersToRemove.Add(character.gameObject);
    }

    public override void OnFocusLost()
    {
        foreach (BaseInput inp in refInput)
        {
            inp.OnInputExecuted -= player.InterpretInput;
            inp.OnInputExecuted -= Inp_OnInputExecuted;
        }
    }

    /// <summary>
    /// On a battu tous les ennemies , niveau suivant
    /// </summary>
    public void GeneratorDone()
    {
        countGenerator--;
        if (countGenerator == 0)
        {
            if (NextLevel != null)
            {
                player.NextLevel += () => { OnNextLevel(NextLevel); };
                WaypointElement we = new WaypointElement();
                we.speed = 1;
                we.targetPosition = Utils.GetPositionAbsolute(new Vector3(0.5f, 0, 0.5f), Mathf.Abs(Camera.main.transform.position.y - player.transform.position.y));
                Queue<WaypointElement> queue = new Queue<WaypointElement>();
                queue.Enqueue(we);
                State st = new FollowPathMovement(player, queue, () => player.SetState(new IdleTransition(player)));
                player.StartDelayedState(1, st);
            }
        }
    }

    public void Menu()
    {
        Constants.ApplicationQuit = true;
        Constants.SetAllConstants(0);
        Utils.StartFading(1f, Color.black, () => SceneManager.LoadScene("Menu"), () => { Constants.SetAllConstants(1); Constants.ApplicationQuit = false; });
    }

    public void Restart()
    {
        Constants.ApplicationQuit = true;
        Utils.StartFading(1f, Color.black, () => SceneManager.LoadScene(SceneManager.GetActiveScene().name), () => { Constants.SetAllConstants(1);Constants.ApplicationQuit = false; });
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void PlayerDied(Character chara)
    {
        Debug.Log("Le joueur est mort");
    }

}
