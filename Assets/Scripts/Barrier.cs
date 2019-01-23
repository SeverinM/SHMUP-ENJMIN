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

    bool alreadyHit = false;
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
        alreadyHit = false;
        if (other.gameObject.tag == Constants.BULLET_TAG || (other.gameObject.tag == Constants.SHIELD_TAG && IsWinching))
        {
            Manager.GetInstance().ShakeCamera(screenShakeBarrierForce, screenShakeDuration);
            Destroy(other.gameObject);
            AkSoundEngine.PostEvent("S_HitShield", gameObject);
            if (other.gameObject.tag == Constants.SHIELD_TAG)
                alreadyHit = true;
        }

        bool isBob = (other.transform.parent != null && other.transform.parent.GetComponent<Enemy>() != null && other.transform.parent.GetComponent<Enemy>().enemyType == Enemy.EnemyType.BOB);
        if ((isBob || other.gameObject.tag == Constants.PULLABLE_TAG || other.gameObject.tag == Constants.WINCHABLE_TAG) && IsWinching && !alreadyHit)
        {
            Manager.GetInstance().ShakeCamera(screenShakeBarrierForce, screenShakeDuration);
            Character chara = other.transform.parent.GetComponent<Character>();
            if (!chara.Context.ValuesOrDefault<bool>(Constants.IN_RECOVERY,false))
            {
                chara.StartRecovery(barrierRecovery);
                chara.PersonalScale = 1;
                Vector3 delta = other.transform.position - transform.position;
                delta.y = 0;
                chara.Hit(delta.normalized * 100);
                Destroy(other.gameObject);
            }
        }
    }
}
