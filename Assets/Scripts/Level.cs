using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : Layers {

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

    public List<GameObject> characters = new List<GameObject>();

    internal Vector2 maxBounds = new Vector2(-8,8);
    internal Vector2 minBounds = new Vector2(-8,8);

    public override void OnFocusGet()
    {      
        foreach(BaseInput inp in refInput)
        {
            inp.OnInputExecuted += player.InterpretInput;
        }

        if (generators.Count == 0)
        {
            return;
        }

        // Setup all generators
        foreach (GameObject generator in generators)
        {
            //Le generateur lit ses propres vagues
            if (generator.GetComponent<Generator>().AllWaves.Count > 0)
            {
                generator.GetComponent<Generator>().SetState(new GenerateEnemies(generator.GetComponent<Generator>(), generator.GetComponent<Generator>().AllWaves));
            }
        }

    }

    /// <summary>
    /// Add a Character to the level
    /// The character will know about the other characters in the level
    /// At the moment it's used for enemies to avoid walkin over each other
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

        GameObject toAdd = Instantiate(character, position, Quaternion.identity);
        toAdd.GetComponent<Enemy>().Level = this;
        toAdd.GetComponent<Enemy>().player = Player;
        characters.Add(toAdd);

        //On cable des evenements
        toAdd.GetComponent<Enemy>().Destroyed += Remove;
        toAdd.GetComponent<Enemy>().TryReachingPlayer += Level_TryReachingPlayer;
        return toAdd;
    }

    private Player Level_TryReachingPlayer()
    {
        return player;
    }

    /// <summary>
    /// Remove a character from the list so it can't be updated anymore
    /// </summary>
    /// <param name="character"></param>
    public void Remove(Character character)
    {
        characters.Remove(character.gameObject);
    }

    public override void OnFocusLost()
    {
        foreach (BaseInput inp in refInput)
        {
            inp.OnInputExecuted -= player.InterpretInput;
        }
    }

}
