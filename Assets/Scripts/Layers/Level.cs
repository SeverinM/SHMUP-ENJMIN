using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using TMPro;
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

    int countFocus = 0;

    public GameObject Player
    {
        get
        {
            return player.gameObject;
        }
    }

    [SerializeField]
    GameObject LifeUI;

    [SerializeField]
    GameObject LifePrefab;

    [SerializeField]
    GameObject CountEnemyUI;

    [SerializeField]
    GameObject CountEnemyPrefab;

    [SerializeField]
    GameObject DashUI;

    [SerializeField]
    GameObject DashPrefab;

    [SerializeField]
    Text ScoreUI;

    [SerializeField]
    Text BonusUI;

    [SerializeField]
    GameObject canvas;

    [SerializeField]
    AnimationCurve textAnimation;

    [SerializeField]
    GameObject textDefault;

    [SerializeField]
    GameObject textMeshProDefault;

    [SerializeField]
    Menu menu;

    Navigation nav;

    int countGenerator = 0;

    private int score = 0;

    private int bonus = 0;

    private float textDuration = 1.0f;

    private float bonusDuration = 2.0f;
    private float timeBonus;


    Dictionary<GameObject, Text> characterTexts = new Dictionary<GameObject, Text>();
    Dictionary<GameObject, Vector3> StoredValue = new Dictionary<GameObject, Vector3>();


    [HideInInspector]
    public List<GameObject> characters = new List<GameObject>();

    [HideInInspector]
    public Binding<int> watchNbSpawn = new Binding<int>(0, null);
    public Binding<int> watchScore = new Binding<int>(0, null);

    public List<Enemy> enemiesOnBonus = new List<Enemy>();

    public delegate void LevelParam(Level nextLevel);
    public event LevelParam OnNextLevel;



    //Appellé quand le layer est au dessus de la stack
    public override void OnFocusGet()
    {
        // Faire en sorte que tous les inputs notifient le joueur
        foreach (BaseInput inp in refInput)
        {
            inp.OnInputExecuted += player.InterpretInput;
            inp.OnInputExecuted += Inp_OnInputExecuted;
        }

        //On arrete là si la couche a deja été mis en avant
        if (countFocus > 0)
        {
            return;
        }

        player.Destroyed += PlayerDied;

        //Mise en place des data bindings;
        player.SetOnLifeChanged((x) =>
        {
            if (LifeUI != null)
            {
                for (int i = 0; i < LifeUI.transform.childCount; i++)
                {
                    Destroy(LifeUI.transform.GetChild(i).gameObject);
                }
                for (int i = 0; i < x; i++)
                {
                    Instantiate(LifePrefab, LifeUI.transform);
                }
            }
        });

        player.SetOnDashChanged((x) =>
        {
            if (DashUI != null)
            {
                for (int i = 0; i < DashUI.transform.childCount; i++)
                {
                    Destroy(DashUI.transform.GetChild(i).gameObject);
                }
                for (int i = 0; i < x; i++)
                {
                    Instantiate(DashPrefab, DashUI.transform);
                }
            }
        });

        watchNbSpawn.ValueChanged = (x) =>
        {
            if (CountEnemyUI != null)
            {
                for (int i = 0; i < CountEnemyUI.transform.childCount; i++)
                {
                    Destroy(CountEnemyUI.transform.GetChild(i).gameObject);
                }
                for (int i = 0; i < x; i++)
                {
                    Instantiate(CountEnemyPrefab, CountEnemyUI.transform);
                }
            }
        };

        watchScore.ValueChanged = (x) =>
        {
            if (ScoreUI != null)
                ScoreUI.text = "Score : " + x.ToString();

            Constants.TotalScore = x;
        };

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
        countFocus++;

    }

    private void Inp_OnInputExecuted(BaseInput.TypeAction tyAct, BaseInput.Actions acts, Vector2 values)
    {
        //On alterne le resultat de l'inputs
        if (tyAct.Equals(BaseInput.TypeAction.Down) && acts.Equals(BaseInput.Actions.Pause))
        {
            Manager.GetInstance().EnableMenu(menu);
        }
    }

    private void Generator_WaveCleaned(int nb, Generator who)
    {
        LockWaveElement elt = new LockWaveElement();
        elt.generator = who;
        elt.number = nb;
        foreach (Generator gen in transform.GetComponentsInChildren<Generator>().Where(x => x != who))
        {
            gen.RemoveLock(elt);
        }
    }

    public void Update()
    {
        watchNbSpawn.WatchedValue = transform.GetComponentsInChildren<Generator>().Select(x => x.GetComponent<Generator>().EnnemiesLeftToSpawn).Sum();

        // Bonus
        timeBonus += Time.deltaTime;
        if (timeBonus > bonusDuration)
        {
            timeBonus = 0f;
            ComboScore();
            enemiesOnBonus.Clear();
        }
    }

    /// <summary>
    /// Un personnage pour calculer le scoring
    /// </summary>
    /// <param name="chara"></param>
    internal void AddCharacterForScore(Character chara)
    {
        score += chara.GetComponent<Enemy>().KillScore; // Score on destruction

        enemiesOnBonus.Add(chara.GetComponent<Enemy>());

        PopScore(chara, chara.GetComponent<Enemy>().KillScore); // Draw score over ennemy head

        timeBonus = 0f; // Combo
        watchScore.WatchedValue = score; // Update Value
    }

    /// <summary>
    /// Calucul des combos
    /// 10 points de plus par Bob détruit 
    /// 20 points de plus par Jim détruit
    /// 40 points de plus par Mike détruit
    /// 3 ennemis = bonus doublé
    /// 6 ennemis = bonus triplé
    /// 9 ennemis = bonus quadruplé
    /// </summary>
    private void ComboScore()
    {
        int total = 0;
        foreach (Enemy e in enemiesOnBonus)
        {
            switch (e.enemyType)
            {
                case Enemy.EnemyType.BOB:
                    total += 10;
                    break;
                case Enemy.EnemyType.JIM:
                    total += 20;
                    break;
                case Enemy.EnemyType.MIKE:
                    total += 40;
                    break;
            }
        }

        if (enemiesOnBonus.Count >= 9)
        {
            total *= 4;
        }
        else if (enemiesOnBonus.Count >= 6)
        {
            total *= 3;
        }
        else if (enemiesOnBonus.Count >= 3)
        {
            total *= 2;
        }

        score += total;
        watchScore.WatchedValue = score; // Update Value
    }

    /// <summary>
    /// Afficher un score au dessus d'un ennemi
    /// </summary>
    /// <param name="chara"></param>
    /// <param name="killScore"></param>
    internal void PopScore(Character chara, int killScore)
    {
        TextMeshProUGUI toAddText = Instantiate(textMeshProDefault, canvas.transform).GetComponent<TextMeshProUGUI>();

        // Position du texte au dessus d'un gameObject
        Vector3 offsetPos = new Vector3(chara.transform.position.x, chara.transform.position.y, chara.transform.position.z + 0.5f);

        // Calcul de la position à l'écran 
        Vector2 canvasPos;
        Vector2 canvasEndPos;
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(offsetPos);

        // Convertir la position à l'écran vers l'espace du canvas 
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), screenPoint, null, out canvasPos);

        toAddText.text = killScore.ToString();
        toAddText.transform.localPosition = canvasPos;

        canvasEndPos = canvasPos;
        canvasEndPos.y += 10f;

        StartCoroutine(PopScoreCoroutine(toAddText, canvasPos, canvasEndPos));
    }

    /// <summary>
    /// Animation de texte
    /// </summary>
    /// <param name="text"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    IEnumerator PopScoreCoroutine(TextMeshProUGUI text, Vector3 start, Vector3 end)
    {
        float time = 0.0f;
        float movement = 0.0f;
        float alpha = 0.0f;

        while (time <= textDuration)
        {
            // obtenir la distance depuis la courbe d'animation
            movement = textAnimation.Evaluate(time / textDuration);
            alpha = 1f - textAnimation.Evaluate(time / textDuration);

            // lerp postion
            text.transform.localPosition = Vector3.Lerp(start, end, movement);

            Color color = text.color;
            color.a = alpha; // Changer l'alpha de la couleur
            text.color = color;

            time += Time.deltaTime;
            yield return null;
        }

        // Retirer le texte du niveau
        Destroy(text);
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
        if (textDefault != null)
        {
            GameObject toAddText = Instantiate(textDefault, canvas.transform);
        }
        return toAdd;
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

    public void PlayerDied(Character chara)
    {
        Debug.Log("Le joueur est mort");
    }

}
