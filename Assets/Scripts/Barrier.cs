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
            AkSoundEngine.PostEvent("S_HitShield", gameObject);
        }

        if (other.gameObject.tag == "Ennemy" && isWinching)
        {
            Character chara = other.GetComponent<Character>();
            chara.StartRecovery(barrierRecovery);

            Vector3 delta = other.transform.position - transform.position;
            delta.y = 0;
            other.GetComponent<Character>().Hit(delta.normalized * 100);
        }
    }
}
