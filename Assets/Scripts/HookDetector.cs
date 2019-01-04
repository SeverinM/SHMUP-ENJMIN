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
            player.AttachHook(other.transform);
            player.ActualState.NextState();
        }

        if (other.tag == "Winchable")
        {
            player.Context.SetInDictionary("HookMode", PlayerWinch.HookMode.Winch);
            player.AttachHook(other.transform);
            player.ActualState.NextState();
        }

        // Si le joueur touche un bouclier, remetre à zero son Hook
        if (other.tag == "Shield")
        {
            //Enemy enemy = other.gameObject.transform.parent.GetComponent<Enemy>();
            //enemy.SetState(new FreezeMovement(enemy));
            Destroy(other.gameObject);
            player.SetState(new PlayerMovement(player));
        }

    }
}
