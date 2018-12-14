using UnityEngine;
using System.Collections;

public class HookDetector : MonoBehaviour
{

    public GameObject player;


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Hookable")
        {
            player.GetComponent<Hook>().hooked = true;
        }
    }
}
