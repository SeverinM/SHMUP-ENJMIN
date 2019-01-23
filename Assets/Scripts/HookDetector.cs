using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class HookDetector : MonoBehaviour
{
    [SerializeField]
    Player player;

    private void OnTriggerEnter(Collider other)
    {
        List<string> possibleTags = new List<string> { Constants.PULLABLE_TAG, Constants.WINCHABLE_TAG, Constants.ENEMY_TAG, Constants.SHIELD_TAG };
        if (other.transform.parent != null && other.transform.parent.GetComponent<Character>() != null && other.transform.parent.GetComponent<Character>().Context.ValuesOrDefault<bool>(Constants.IN_RECOVERY, false))
        {
            player.SetState(new PlayerMovement(player));
        }

        // Succés Hook
        if (other.tag == Constants.PULLABLE_TAG) 
        {            
            player.Context.SetInDictionary(Constants.HOOK_MODE, PlayerWinch.HookMode.Pull);
            player.Context.SetInDictionary(Constants.POSITION_LAND, transform.position);
            player.AttachHook(other.transform);
            player.NextState();
            AkSoundEngine.PostEvent("H_Grab", gameObject);
        }

        if (other.tag == Constants.WINCHABLE_TAG)
        {
            player.Context.SetInDictionary(Constants.HOOK_MODE, PlayerWinch.HookMode.Winch);
            player.Context.SetInDictionary(Constants.POSITION_LAND, transform.position);
            player.AttachHook(other.transform);
            player.NextState();
            AkSoundEngine.PostEvent("H_Grab", gameObject);
        }

        if (other.tag == Constants.ENEMY_TAG)
        {
            player.SetState(new PlayerMovement(player));
            //AkSoundEngine.PostEvent("H_Grab", gameObject);
        }

        // Si le joueur touche un bouclier, le grab
        if (other.tag == Constants.SHIELD_TAG)
        {
            AkSoundEngine.PostEvent("E3_ShieldGrab", gameObject);
            player.Context.SetInDictionary(Constants.POSITION_LAND, transform.position);
            player.Context.SetInDictionary(Constants.IS_SHIELD, true);
            player.AttachHook(other.transform);
            player.NextState();
        }

    }
}
