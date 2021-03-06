﻿using UnityEngine;
using System.Collections;

/// <summary>
/// Dans cet etat , le joueur est en train de tracter ou d'etre tracté
/// </summary>
public class PlayerWinch : State
{
    private Player player;
    bool isShield;

    Transform barrier;
    
    //Vitesse de traversé du hook
    float speedTravel;
    float hookRadius = 0.1f;
    public enum HookMode
    {
        Pull,
        Winch
    }
    HookMode currentMode;
    Vector3 copy;

    float distanceToHook;
    float minimalDistance;

    public PlayerWinch(Character character) : base(character)
    {
        player = character.GetComponent<Player>();
        barrier = character.Context.ValuesOrDefault<Transform>(Constants.BARRIER, character.transform);
        speedTravel = character.Context.ValuesOrDefault<float>(Constants.SPEED_WINCH, 10);
        currentMode = character.Context.ValuesOrDefault<HookMode>(Constants.HOOK_MODE, HookMode.Winch);
    }

    public override void EndState()
    {
        barrier.GetComponent<Barrier>().IsWinching = false;
        AkSoundEngine.PostEvent("H_Winch_Stop", character.gameObject);
        character.Context.Remove(Constants.IS_SHIELD);
        player.Hook.parent = player.transform;
    }

    public override void NextState()
    {
        // Quand on a atteint notre cible, alors on repasse à l'état normal du joueur
        character.SetState(new PlayerMovement(character));
    }

    public override void StartState()
    {
        isShield = character.Context.ValuesOrDefault<bool>(Constants.IS_SHIELD, false);
        if (isShield)
        {
            currentMode = HookMode.Pull;
            player.Target.parent.GetComponent<Character>().PersonalScale = 1;
            player.Target.transform.parent = null;
        }
        barrier.GetComponent<Barrier>().IsWinching = true;
        AkSoundEngine.PostEvent("H_Winch", character.gameObject);
        player.Hook.parent = null;
    }

    public override void UpdateState()
    {
        if (player.Target == null)
        {
            NextState();
            return;
        }
        //La distance se fait entre le joueur et son hook
        distanceToHook = Vector3.Distance(character.transform.position, player.Hook.transform.position);
        minimalDistance = Mathf.Min(distanceToHook - 0.02f, Time.deltaTime * character.GetScale() * speedTravel * character.PersonalScale);

        if (currentMode == HookMode.Winch)
        {
            // Le joueur se propulse en avant, ce qui fait avancer tous les enfants
            // On utilise copy afin de maintenir le hook à la même position
            copy = player.Hook.transform.position;

            character.transform.position += character.transform.forward * minimalDistance;
        }

        if (currentMode == HookMode.Pull)
        {
            // Puisque l'on est en collision avec l'enfant, on va tirer tout l'ensemble, donc le parent
            Transform who = isShield ? player.Target : player.Target.parent;
            who.position -= character.transform.forward * minimalDistance;
            player.Hook.transform.position -= character.transform.forward * minimalDistance;
        }

        //Si la distance hook / vaisseau est inferieur au radius de hook, retourner vers mouvement
        if (distanceToHook <= hookRadius)
        {
            NextState();
        }

        character.transform.forward = player.Hook.position - character.transform.position;
        character.transform.forward = new Vector3(character.transform.forward.x, 0, character.transform.forward.z);
        player.Hook.transform.position = new Vector3(player.Hook.transform.position.x, character.transform.position.y, player.Hook.transform.position.z);
    }

    public override string GetName()
    {
        return "PlayerWinch";
    }
}
