using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Each enemy can shoot the player according to a shoot period
/// </summary>
public class EnemyAttack : State
{
    private Enemy enemy;

    private float lastTime;
    private float shoots = 0;

    private GameObject player;
    private List<GameObject> characters;

    public EnemyAttack(Character character, List<GameObject> characters, GameObject player) : base(character)
    {
        enemy = character.GetComponent<Enemy>();
        this.player = player;
        lastTime = Time.time;
        this.characters = characters;
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
        character.SetState(new MovementEnemy(character, characters, player, player.transform, MovementEnemy.MovementState.NORMAL));
    }

    public override void StartState()
    {
        base.StartState();
    }

    public override void UpdateState()
    {
        // Attacks according to shootPeriod
        if (lastTime < Time.time)
        {
            lastTime += enemy.shootPeriod;

            enemy.Shoot();
            shoots++;

            if (shoots == enemy.shootAmount)
            {
                NextState();
            }
        }
    }
}
