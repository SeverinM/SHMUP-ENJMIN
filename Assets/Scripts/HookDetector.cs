using UnityEngine;
using System.Collections;


[RequireComponent(typeof(LineRenderer))]
public class HookDetector : MonoBehaviour
{
    [SerializeField]
    Player player;

    private void OnTriggerEnter(Collider other)
    {

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
            player.Context.SetInDictionary("PositionLand", transform.position);
            player.Context.SetInDictionary("IsShield", true);
            player.AttachHook(other.transform);
            player.NextState();
        }

    }
}
