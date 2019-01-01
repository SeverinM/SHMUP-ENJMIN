using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour {

    bool isWinching = false;
    public bool IsWinching
    {
        get
        {
            return isWinching;
        }
        set
        {
            isWinching = value;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            Destroy(other.gameObject);
        }

        if (other.gameObject.tag == "Ennemy" && isWinching)
        {
            other.GetComponent<Character>().Life--;
            other.GetComponent<Character>().StartRecovery(1);
        }
    }
}
