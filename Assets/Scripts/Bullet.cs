using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    [SerializeField]
    private float lasting = 1f;
    private float elapsedTime;

    void Update () {
        elapsedTime += Time.deltaTime;

        if (elapsedTime > lasting)
        {
            Destroy(gameObject);
        }
    }
}
