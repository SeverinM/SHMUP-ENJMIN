using UnityEngine;
using System.Collections;

public class GenerateEnemies : State
{
    private Generator generator;
    private Level level;
    private float lastTime;

    public GenerateEnemies(Character character) : base(character)
    {
        generator = character.GetComponent<Generator>();
        level = generator.levelObject.GetComponent<Level>();
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
        // Generate enemies according to the time period
        if (lastTime < Time.time)
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
