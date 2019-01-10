using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

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
    protected Level NextLevel;

    public GameObject Player
    {
        get
        {
            return player.gameObject;
        }
    }

    [SerializeField]
    Text textUI;

    [SerializeField]
    Text Count;

    [SerializeField]
    GameObject canvas;

    [SerializeField]
    GameObject text;

    int countGenerator = 0;

    Dictionary<GameObject, Text> characterTexts = new Dictionary<GameObject, Text>();

    public List<GameObject> characters = new List<GameObject>();
    public List<GameObject> charactersToRemove = new List<GameObject>();
    public Binding<int> watchNbSpawn = new Binding<int>(0, null);

    public delegate void LevelParam(Level nextLevel);
    public event LevelParam OnNextLevel;

    //Appellé quand le layer est au dessus de la stack
    public override void OnFocusGet()
    {
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

        // Provisoirement
        GameObject toAddText = Instantiate(text, canvas.transform);
        characterTexts.Add(player.gameObject, toAddText.GetComponent<Text>());
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
            //On renvoit successivement des events
            player.StartDelayedState(1, new IdleTransition(player));
            player.NextLevel += () => {
                if (NextLevel != null && OnNextLevel != null)
                    OnNextLevel(NextLevel);
            };
        }
    }

    public void PlayerDied(Character chara)
    {
        Debug.Log("Le joueur est mort");
    }

}
