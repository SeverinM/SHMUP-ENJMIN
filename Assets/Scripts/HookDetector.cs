using UnityEngine;
using System.Collections;

public class HookDetector : MonoBehaviour
{
    [SerializeField]
    Player player;

    private void OnTriggerEnter(Collider other)
    {
        // Success on Hook
        if (other.tag == "Hookable") 
        {
            player.ActualState.NextState();
        }
    }
}
