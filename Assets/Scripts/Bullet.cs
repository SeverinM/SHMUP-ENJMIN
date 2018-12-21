using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    [SerializeField]
    private float speed = 6f;
    private int lastTime;

    [SerializeField]
    private int lasting = 256;

    void Update () {

        if (lastTime < lasting)
        {
            lastTime++;
        } else
        {
            Destroy(gameObject);
        }
    }
}
