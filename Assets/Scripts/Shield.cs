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
            other.GetComponent<Enemy>().StartDelayedTask(() => { Constants.TimeScaleEnnemies = 0; Constants.TimeScalePlayer = 0; },
                0.2f,
                () => { Constants.TimeScaleEnnemies = 1; Constants.TimeScalePlayer = 1; });
        }
    }
}
