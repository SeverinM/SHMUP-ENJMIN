using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Character {

    public float Duration = 1;
    float time = 0;

    public override float GetScale()
    {
        return Constants.TimeScaleBullet;
    }

    private new void Update()
    {
        time += Time.deltaTime * GetScale();
        if (time > Duration)
        {
            Destroy(gameObject);
        }
    }

}
