using UnityEngine;
using System.Collections;

/// <summary>
/// The generators will Instanciate ennemies in the level according to a period
/// </summary>
public class GenerateEnemies : State
{
    private Generator generator;
    private Level level;
    private float lastTime;

    public GenerateEnemies(Character character) : base(character)
    {
        generator = character.GetComponent<Generator>();
        level = generator.levelObject.GetComponent<Level>();
        lastTime = Time.time;
    }

    public override void EndState()
    {
        base.EndState();
    }

    public override void InterpretInput(BaseInput.TypeAction typeAct, BaseInput.Actions acts, Vector2 val)
    {
        base.InterpretInput(typeAct, acts, val);
    }

    public override void NextState()
    {
        base.NextState();
    }

    public override void StartState()
    {
        base.StartState();
    }

    public override void UpdateState()
    {
        if (lastTime < Time.time) // Generate enemies according to the time period
        {
            lastTime += generator.period;

            if (generator.waves > 0 || generator.waves == -1)
            {
                level.Instanciate(generator.enemyPrefab, character.transform.position);
                generator.waves--;
            }
        }
    }
}
