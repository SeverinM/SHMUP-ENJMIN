using UnityEngine;
using System.Collections;


[RequireComponent(typeof(LineRenderer))]
public class HookDetector : MonoBehaviour
{
    [SerializeField]
    Player player;

    private void OnTriggerEnter(Collider other)
    {
        // Success on Hook
        if (other.tag == "Pullable") 
        {
            //GetComponent<LineRenderer>().SetPosition(0, transform.position);
            player.Context.SetInDictionary("HookMode", PlayerWinch.HookMode.Pull);
            player.AttatchHook(other.transform);
            player.ActualState.NextState();
        }

        if (other.tag == "Winchable")
        {
            //GetComponent<LineRenderer>().SetPosition(0, transform.position);
            player.Context.SetInDictionary("HookMode", PlayerWinch.HookMode.Winch);
            player.ActualState.NextState();
        }
    }
}
