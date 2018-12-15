using UnityEngine;
using System.Collections;

public class HookDetector : MonoBehaviour
{

    public GameObject playerGo;
    public GameObject hook;

    private Player player;

    private void Start()
    {
        player = playerGo.GetComponent<Player>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Success on Hook
        if (other.tag == "Hookable") 
        {
            player.SetState(new PlayerWinch(player, hook));
        }
    }
}
