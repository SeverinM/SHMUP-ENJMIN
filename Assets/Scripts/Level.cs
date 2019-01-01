using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public GameObject Player
    {
        get
        {
            return player.gameObject;
        }
    }


    [SerializeField]
    List<GameObject> generators;

    [SerializeField]
    GameObject canvas;


    [SerializeField]
    GameObject text;


    Dictionary<GameObject, Text> characterTexts = new Dictionary<GameObject, Text>();


    public List<GameObject> characters = new List<GameObject>();
    public List<GameObject> charactersToRemove = new List<GameObject>();

    internal Vector2 maxBounds = new Vector2(-8, 8);
    internal Vector2 minBounds = new Vector2(-8, 8);


    public override void OnFocusGet()
    {
        // Set level in order for player to remove himself
        player.level = this;

        // Faire en sorte que tous les inputs notifient le joueur
        foreach (BaseInput inp in refInput)
        {
            inp.OnInputExecuted += player.InterpretInput;
        }

        if (generators.Count == 0)
        {
            return;
        }

        // Mettre en route tous les générateurs en leur attribuant un état
        foreach (GameObject generator in generators)
        {
            //Le generateur lit ses propres vagues
            if (generator.GetComponent<Generator>().AllWaves.Count > 0)
            {
                generator.GetComponent<Generator>().SetState(new GenerateEnemies(generator.GetComponent<Generator>(), generator.GetComponent<Generator>().AllWaves));
            }
        }

    }

    public void Update()
    {
        foreach (GameObject character in charactersToRemove)
        {
            characterTexts.Remove(character);
            characters.Remove(character);
            Destroy(character);
        }

    }

    public void LateUpdate()
    {
        foreach (KeyValuePair<GameObject, Text> value in characterTexts)
        {
            // Affichage de données depuis le GameObject
            value.Value.text = value.Key.GetComponent<Enemy>().ActualState.ToString();

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

    /// <summary>
    /// Ajouter un Personnage au niveau
    /// Cela permet au niveau de connaitre tous les ennemis instanciés
    /// </summary>
    /// <param name="character"></param>
    /// <param name="position"></param>
    public GameObject Instanciate(Enemy.EnemyType type, Vector3 position)
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

        // Instantier un personnage
        GameObject toAdd = Instantiate(character, position, Quaternion.identity);
        toAdd.GetComponent<Enemy>().level = this;
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
    /// Retirer un personnage de la liste
    /// </summary>
    /// <param name="character"></param>
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

}
