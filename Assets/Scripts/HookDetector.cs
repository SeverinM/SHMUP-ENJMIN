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
        if (other.tag == "Hookable") 
        {
            GetComponent<LineRenderer>().SetPosition(0, transform.position);
            player.ActualState.NextState();
        }
    }
}
