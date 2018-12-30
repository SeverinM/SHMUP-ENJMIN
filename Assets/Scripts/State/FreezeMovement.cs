using UnityEngine;
using System.Collections;

/// <summary>
/// Un ennemi se gèle sur place avec une annimation de froid
/// </summary>
public class FreezeMovement : State
{

    private float shake = 1f;
    private float shakeAmount = 0.1f;
    private float decreaseFactor = 1.0f;

    State movement;
    Level level;

    public FreezeMovement(Character character, State movement) : base(character)
    {
        this.movement = movement;
        level = character.Context.ValuesOrDefault<Level>("Level", null);
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
        if (shake > 0)
        {
            character.transform.position += Random.insideUnitSphere * shakeAmount;
            shake -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            // Détacher le grappin
            level.Player.GetComponent<Player>().SetState(new PlayerMovement(level.Player.GetComponent<Player>()));
            character.SetState(movement);
        }
    
    }
}
