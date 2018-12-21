
using UnityEngine;
using System.Collections;

/// <summary>
/// Will choose a random location for the character to go to
/// If the player is nearby, it will instead start another State to follow the player
/// </summary>
public class WanderMovement : State
{
    private Level level;
    private Transform next;

    GameObject nextPosition = new GameObject();
    Vector3 targetPosition;

    State movement; 

    public WanderMovement(Character character, Level level) : base(character)
    {
        this.level = level;

        NextPosition();
       
        movement = new EnemyMovement(character, level, nextPosition.transform);
    }

    public override void InterpretInput(BaseInput.TypeAction typeAct, BaseInput.Actions acts, Vector2 val)
    {

    }

    public void NextPosition()
    {
        nextPosition.transform.Translate(Random.Range(level.minBounds.x, level.minBounds.y), 0, Random.Range(level.maxBounds.x, level.maxBounds.y));
    }

    public override void UpdateState()
    {
        movement.UpdateState();
        
    }

    public override void EndState()
    {
        //Fin de l'etat, on a plus besoin de connaitre les triggers
        character.OnTriggerEnterChar -= TriggerEnter;
    }

    public override void NextState()
    {
        character.SetState(null);
    }

    public override void StartState()
    {
        //On veut savoir si le personnage touche un certain trigger
        character.OnTriggerEnterChar += TriggerEnter;
    }

    public void TriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag == "FollowParent")
        {
            character.SetState(new EnemyMovement(character, level, coll.transform.parent));
        }
    }

}
