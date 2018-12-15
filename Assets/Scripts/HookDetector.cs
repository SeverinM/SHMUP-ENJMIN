using UnityEngine;
using System.Collections;

public class HookDetector : MonoBehaviour
{
    [SerializeField]
    Player player;

    [SerializeField]
    GameObject hook;

    private void OnTriggerEnter(Collider other)
    {
        // Success on Hook
        if (other.tag == "Hookable") 
        {
            player.SetState(new PlayerWinch(player, hook));
        }
    }
}
