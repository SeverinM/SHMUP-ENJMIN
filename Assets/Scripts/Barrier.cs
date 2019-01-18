using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour {

    [SerializeField]
    private float barrierRecovery = 1f;

    [SerializeField]
    private int lifeHit = 1;

    [SerializeField]
    private float screenShakeDuration = 0.0012f;


    [SerializeField]
    private float screenShakeForce = 0.5f;

    [SerializeField]
    private float screenShakeBarrierForce = 1f;


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
            Manager.GetInstance().ShakeCamera(screenShakeForce, screenShakeDuration);
            Destroy(other.gameObject);
            AkSoundEngine.PostEvent("S_HitShield", gameObject);
        }

        if (other.gameObject.tag == "Ennemy" && isWinching)
        {
            Manager.GetInstance().ShakeCamera(screenShakeBarrierForce, screenShakeDuration);
            other.GetComponent<Enemy>().Life -= lifeHit;
            other.GetComponent<Enemy>().StartRecovery(barrierRecovery);
        }
    }
}
