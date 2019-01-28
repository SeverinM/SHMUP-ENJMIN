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

    [Header("Objets du niveau")]
    [SerializeField]
    protected Player player;

    [SerializeField]
    protected GameObject JimPrefab;

    [SerializeField]
    protected GameObject MikePrefab;

    [SerializeField]
    protected GameObject BobPrefab;

    int countFocus = 0;

    public Player Player
    {
        get
        {
            return player;
        }
    }

    [SerializeField]
    [Tooltip("Arriere-plan du niveau precedent a cacher")]
    GameObject backgroundToHide;
    public GameObject BackgroundToHide
    {
        get
        {
            return backgroundToHide;
        }
    }

    [Header("Objets de l'interface")]
    [SerializeField]
    GameObject canvas;

    [SerializeField]
    [Tooltip("Layer pause")]
    Menu pauseMenu;

    [SerializeField]
    [Tooltip("Canvas pour l'affichage de la vie du joueur")]
    GameObject LifeUI;

    [SerializeField]
    [Tooltip("Prefab du sprite de vie du joueur")]
    GameObject LifePrefab;

    [SerializeField]
    [Tooltip("Texte personalise d'affichage du score")]
    TextMeshProUGUI ScoreUI;

    [SerializeField]
    [Tooltip("Texte personalise d'affichage des bonus")]
    TextMeshProUGUI BonusUI;

    [SerializeField]
    [Tooltip("Texte personalise pour affichage du score d'un ennemi lors de sa mort")]
    GameObject textMeshProDefault;

    [Header("Animations")]
    [SerializeField]
    [Tooltip("Animation a lancer au debut d'un niveau")]
    private Animator levelStartAnimator;

    [SerializeField]
    [Tooltip("Animation de soin de vie interface")]
    private Animator animationHeal;

    [SerializeField]
    [Tooltip("Animation d'appation de texte de score a la mort d'un ennemi")]
    private AnimationCurve textAnimation;

    private Navigation nav;

    int countGenerator = 0;
    int bonus = 0;
    float textDuration = 1.0f;
    float bonusDuration = 2.0f;
    float timeBonus;

    Dictionary<GameObject, Text> characterTexts = new Dictionary<GameObject, Text>();
    Dictionary<GameObject, Vector3> StoredValue = new Dictionary<GameObject, Vector3>();

    [HideInInspector]
    public List<GameObject> characters = new List<GameObject>();

    [HideInInspector]
    public Binding<int> watchNbSpawn = new Binding<int>(-1, null);
    public Binding<int> watchScore = new Binding<int>(0, null);

    private List<Enemy> enemiesOnBonus = new List<Enemy>();

    public delegate void LevelParam(Level nextLevel);
    public event LevelParam OnNextLevel;

    int previousLife;

    //Appellé quand le layer est au dessus de la stack
    public override void OnFocusGet()
    {
        watchScore.WatchedValue = Constants.TotalScore;

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

        if (IsFirst)
            AkSoundEngine.PostEvent("Level_01_Start", gameObject);

        TryPlace();

        Constants.ApplicationQuit = false;
        player.Destroyed += PlayerDied;

        //Mise en place des data bindings
        player.SetOnLifeChanged((x) =>
        {
            if (LifeUI != null)
            {
                // On nettoye tous les prefabs de vie de l'UI
                for (int i = 0; i < LifeUI.transform.childCount; i++)
                {
                    Destroy(LifeUI.transform.GetChild(i).gameObject);
                }

                // On instancie selon la vie du joeur (x)
                for (int i = 0; i < x; i++)
                {
                    Instantiate(LifePrefab, LifeUI.transform);
                }

                // Animation de heal
                if (x > previousLife)
                {
                    animationHeal.SetTrigger("Healed");
                }
            }
        });

        Manager.GetInstance().ResetLife();

        // Mettre à jour l'affichage du score
        watchScore.ValueChanged = (x) =>
        {
            if (ScoreUI != null)
                ScoreUI.text = "Score : " + x.ToString();

            Constants.TotalScore = x;
        };

        watchNbSpawn.ValueChanged = (x) =>
        {
            if (x == 0)
            {
                EndLevel();
            }
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
            generator.WaveCleaned += Generator_WaveCleaned;
        }

        // Lancer les animations de début de niveau
        if (levelStartAnimator != null)
        {
            levelStartAnimator.SetTrigger("Start");
        }

        watchNbSpawn.WatchedValue = -1;
        countFocus++;
    }

    void TryPlace()
    {
        // On place le joueur au centre de la postion absolue de la camera
        Camera.main.transform.position = transform.position + Manager.GetInstance().CameraPositionRelative;
        Player plyr = GameObject.FindObjectOfType<Player>();
        float deltaY = Mathf.Abs(plyr.transform.position.y - transform.position.y);
        if (plyr && deltaY > 0.1f)
        {
            Vector3 camPos = Utils.GetPositionAbsolute(new Vector3(0.5f, 0, 0.5f), Mathf.Abs(transform.position.y - Camera.main.transform.position.y));
            plyr.transform.position = camPos;
        }
    }

    private void Inp_OnInputExecuted(BaseInput.TypeAction tyAct, BaseInput.Actions acts, Vector2 values)
    {
        //On alterne le resultat de l'inputs
        if (tyAct.Equals(BaseInput.TypeAction.Down) && acts.Equals(BaseInput.Actions.Pause))
        {
            Manager.GetInstance().EnableMenu(pauseMenu);
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

        // Fin du combo
        timeBonus += Time.deltaTime;
        if (timeBonus > bonusDuration)
        {
            timeBonus = 0f;
            enemiesOnBonus.Clear();
            AkSoundEngine.SetRTPCValue("Var_Combo_Count", enemiesOnBonus.Count);
        }

        previousLife = player.Life;
        watchScore.WatchedValue = Constants.TotalScore;
    }

    /// <summary>
    /// Un personnage pour calculer le scoring
    /// </summary>
    /// <param name="chara"></param>
    internal void AddCharacterForScore(Character chara)
    {
        watchScore.WatchedValue += chara.GetComponent<Enemy>().KillScore; // Score on destruction

        PopScore(chara, chara.GetComponent<Enemy>().KillScore); // Draw score over ennemy head
        enemiesOnBonus.Add(chara.GetComponent<Enemy>());
        AkSoundEngine.SetRTPCValue("Var_Combo_Count", enemiesOnBonus.Count);

        timeBonus = 0f; // Combo
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
    private int ComboScore(Enemy enn)
    {
        if (enemiesOnBonus.Count == 0)
        {
            return enn.KillScore;
        }

        int total = 0;
        switch (enn.enemyType)
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

        return total + enn.KillScore;
    }

    /// <summary>
    /// Afficher un score au dessus d'un ennemi
    /// </summary>
    /// <param name="chara"></param>
    /// <param name="killScore"></param>
    internal void PopScore(Character chara, int killScore)
    {
        TextMeshProUGUI toAddText = Instantiate(textMeshProDefault, canvas.transform).GetComponent<TextMeshProUGUI>();

        //// Position du texte au dessus d'un gameObject
        Vector3 offsetPos = new Vector3(chara.transform.position.x, chara.transform.position.y, chara.transform.position.z + 0.5f);

        //// Calcul de la position à l'écran 
        Vector2 canvasPos;
        Vector2 canvasEndPos;
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(offsetPos);

        //// Convertir la position à l'écran vers l'espace du canvas 
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), screenPoint, null, out canvasPos);

        toAddText.text = ComboScore(chara.GetComponent<Enemy>()).ToString();
        toAddText.color = Color.Lerp(Color.white, Color.red, enemiesOnBonus.Count / 9);
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
        return toAdd;
    }

    public override void OnFocusLost()
    {
        foreach (BaseInput inp in refInput)
        {
            if (player != null)
                inp.OnInputExecuted -= player.InterpretInput;

            inp.OnInputExecuted -= Inp_OnInputExecuted;
        }
    }

    public void EndLevel()
    {
        if (nextLevel != null)
        {
            player.NextLevel += () => { OnNextLevel(nextLevel); };
            player.BlackScreen += () => {
                if (backgroundToHide != null)
                    Destroy(backgroundToHide);
            };
            WaypointElement we = new WaypointElement();
            we.speed = 1;
            we.targetPosition = Utils.GetPositionAbsolute(new Vector3(0.5f, 0, 0.5f), Mathf.Abs(Camera.main.transform.position.y - player.transform.position.y));
            Queue<WaypointElement> queue = new Queue<WaypointElement>();
            queue.Enqueue(we);
            State st = new FollowPathMovement(player, queue, () => player.SetState(new IdleTransition(player)));
            player.StartDelayedState(1, st);
        }

        else
        {
            StartCoroutine(DelayedEnd());
        }
    }

    IEnumerator DelayedEnd()
    { 
        yield return new WaitForSeconds(3f);
        Utils.StartFading(1, Color.black, () => { End(); }, () => { });
    }

    public void End()
    {
        Manager.GetInstance().PostScore();
        SceneManager.LoadScene("End");
    }

    public void PlayerDied(Character chara)
    {
    }
}
