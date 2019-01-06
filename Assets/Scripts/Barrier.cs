using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour {

    [SerializeField]
    private float barrierRecovery = 1f;

    [SerializeField]
    private int lifeHit = 1;


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
            other.GetComponent<Character>().Life -= lifeHit;
            other.GetComponent<Character>().StartRecovery(barrierRecovery);
        }
    }
}
