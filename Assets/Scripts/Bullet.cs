using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    private float speed = 6f;

	void Update () {
        transform.Translate(transform.forward * Time.deltaTime * speed);

    }
}
