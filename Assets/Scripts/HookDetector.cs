using UnityEngine;
using System.Collections;

public class HookDetector : MonoBehaviour
{

    public GameObject player;
    public GameObject hook;


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Hookable")
        {
            player.GetComponent<Player>().SetState(new PlayerHook(player.GetComponent<Player>(), hook));
        }
    }
}
