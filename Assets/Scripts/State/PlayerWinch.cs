using UnityEngine;
using System.Collections;

/// <summary>
/// Dans cet etat , le joueur est en train de tracter ou d'etre tracté
/// </summary>
public class PlayerWinch : State
{
    private Player player;

    Transform shield;
    
    //Vitesse de traversé du hook
    float speedTravel;

    float hookRadius = 0.75f;

    public enum HookMode
    {
        Pull,
        Winch
    }
    HookMode currentMode;

    Vector3 copy;

    public PlayerWinch(Character character) : base(character)
    {
        player = character.GetComponent<Player>();
        shield = character.Context.ValuesOrDefault<Transform>("Shield", character.transform);
        speedTravel = character.Context.ValuesOrDefault<float>("SpeedWinch", 10);
        currentMode = character.Context.ValuesOrDefault<HookMode>("HookMode", HookMode.Winch);
    }

    public override void EndState()
    {
        shield.GetComponent<Shield>().IsWinching = false;
        if (player.Target != null)
        {
            player.Target.parent.GetComponent<Character>().PersonalScale = 1;
        }      
    }

    public override void NextState()
    {
        // Quand on a atteint notre cible, alors on repasse à l'état normal du joueur
        character.SetState(new PlayerMovement(character));
    }

    public override void StartState()
    {
        shield.GetComponent<Shield>().IsWinching = true;
    }

    public override void UpdateState()
    {
        if (currentMode == HookMode.Winch)
        {
            // Le joueur se propulse en avant, ce qui fait avancer tous les enfants
            // On utilise copy afin de maintenir le hook à la même position
            copy = player.Hook.transform.position;
            character.transform.position += character.transform.forward * Time.deltaTime * character.GetScale() * speedTravel;
            player.Hook.transform.position = copy;
        }

        if (currentMode == HookMode.Pull)
        {
            // Puisque l'on est en collision avec l'enfant, on va tirer tout l'ensemble, donc le parent
            player.Target.parent.transform.position -= character.transform.forward * Time.deltaTime * character.GetScale() * speedTravel;
            player.Hook.transform.position -= character.transform.forward * Time.deltaTime * character.GetScale() * speedTravel;
        }

        float distanceToHook = Vector3.Distance(character.transform.position, player.Hook.transform.position);

        //Si la distance hook / vaisseau est inferieur au radius de hook, retourner vers mouvement
        if (distanceToHook <= hookRadius)
        {
            NextState();

            //Si le pere est un ennemie , le faire passer dans l'etat suivant
            if (player.Target != null)
            {
                player.Target.parent.GetComponent<Enemy>().ActualState.NextState();
            }
        }
    }
}
