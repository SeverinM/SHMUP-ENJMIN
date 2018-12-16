using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : Layers {

    [SerializeField]
    protected Player player;

    [SerializeField]
    List<GameObject> generators;

    private List<GameObject> characters = new List<GameObject>();

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

    public void Instanciate(GameObject character, Vector3 position)
    {
        GameObject toAdd = Instantiate(character, position, Quaternion.identity);
        toAdd.GetComponent<Enemy>().player = player.gameObject;
        characters.Add(toAdd);
    }

    public override void OnFocusLost()
    {
        foreach (BaseInput inp in refInput)
        {
            inp.OnInputExecuted -= player.InterpretInput;
        }
    }

    public void Start()
    {

    }
}
