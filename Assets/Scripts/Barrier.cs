using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour {

    [SerializeField]
    private float barrierRecovery = 1f;

    [SerializeField]
    private int lifeHit = 1;

    [SerializeField]
    private float screenShakeDuration;

    [SerializeField]
    private float screenShakeForce;

    [SerializeField]
    private float screenShakeBarrierForce;


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
        if (other.gameObject.tag == "Bullet" || (other.gameObject.tag == "Shield" && IsWinching))
        {
            Manager.GetInstance().ShakeCamera(screenShakeBarrierForce, screenShakeDuration);
            Destroy(other.gameObject);
            AkSoundEngine.PostEvent("S_HitShield", gameObject);
        }

        if (other.gameObject.tag == "Ennemy" && isWinching)
        {
            Manager.GetInstance().ShakeCamera(screenShakeBarrierForce, screenShakeDuration);
            Character chara = other.GetComponent<Character>();
            chara.StartRecovery(barrierRecovery);

            Vector3 delta = other.transform.position - transform.position;
            delta.y = 0;
            other.GetComponent<Character>().Hit(delta.normalized * 100);
        }
    }
}
