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
        List<string> possibleTags = new List<string> { "Pullable", "Winchable", "Ennemy", "Shield" };
        if (other.transform.parent != null && other.transform.parent.GetComponent<Character>() != null && other.transform.parent.GetComponent<Character>().Context.ValuesOrDefault<bool>("InRecovery", false))
        {
            player.SetState(new PlayerMovement(player));
        }

        // Succés Hook
        if (other.tag == "Pullable") 
        {            
            player.Context.SetInDictionary("HookMode", PlayerWinch.HookMode.Pull);
            player.Context.SetInDictionary("PositionLand", transform.position);
            player.AttachHook(other.transform);
            player.NextState();
            AkSoundEngine.PostEvent("H_Grab", gameObject);
        }

        if (other.tag == "Winchable")
        {
            player.Context.SetInDictionary("HookMode", PlayerWinch.HookMode.Winch);
            player.Context.SetInDictionary("PositionLand", transform.position);
            player.AttachHook(other.transform);
            player.NextState();
            AkSoundEngine.PostEvent("H_Grab", gameObject);
        }

        if (other.tag == "Ennemy")
        {
            player.SetState(new PlayerMovement(player));
            AkSoundEngine.PostEvent("H_Grab", gameObject);
        }

        // Si le joueur touche un bouclier, le grab
        if (other.tag == "Shield")
        {
            AkSoundEngine.PostEvent("E3_ShieldGrab", gameObject);
            player.Context.SetInDictionary("PositionLand", transform.position);
            player.Context.SetInDictionary("IsShield", true);
            player.AttachHook(other.transform);
            player.NextState();
        }

    }
}
