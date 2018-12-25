using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : Layers {

    [SerializeField]
    protected Player player;

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
            generator.GetComponent<Generator>().SetState(new GenerateEnemies(generator.GetComponent<Generator>()));
        }

    }

    /// <summary>
    /// Add a Character to the level
    /// The character will know about the other characters in the level
    /// At the moment it's used for enemies to avoid walkin over each other
    /// </summary>
    /// <param name="character"></param>
    /// <param name="position"></param>
    public void Instanciate(GameObject character, Vector3 position)
    {
        GameObject toAdd = Instantiate(character, position, Quaternion.identity);
        toAdd.GetComponent<Enemy>().level = this;
        toAdd.GetComponent<Enemy>().player = Player;
        characters.Add(toAdd);
    }

    /// <summary>
    /// Remove a character from the list so it can't be updated anymore
    /// </summary>
    /// <param name="character"></param>
    public void Remove(GameObject character)
    {
        characters.Remove(character);
        Destroy(character);
    }

    public override void OnFocusLost()
    {
        foreach (BaseInput inp in refInput)
        {
            inp.OnInputExecuted -= player.InterpretInput;
        }
    }

}
