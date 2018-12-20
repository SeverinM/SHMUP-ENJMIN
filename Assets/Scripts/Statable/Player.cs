using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Player : Character
{
 
    public enum MovementMode
    {
        Dash,
        Normal
    }

    [SerializeField]
    Transform shield;
    public Transform Shield
    {
        get
        {
            return shield;
        }
    }

    [SerializeField]
    MovementMode mode;
    public MovementMode Mode
    {
        get
        {
            return mode;
        }
    }

    [SerializeField]
    private Transform hook;
    public Transform Hook
    {
        get
        {
            return hook;
        }
    }

    void Start()
    {
        Context ctx = new Context();
        ctx.SetInDictionary("Mode", mode);
        ctx.SetInDictionary("Hook", hook);
        ctx.SetInDictionary("Shield", shield);
        actualState = new PlayerMovement(this, ctx);
    }

    new void Update()
    {
        if (actualState != null)
        {
            actualState.UpdateState();
        }

        if (impact.magnitude > 0.2)
        { 
            Move(impact * Time.deltaTime); // move character
        }
        // impact vanishes to zero over time
        impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (other.gameObject.tag == "Ennemy" || other.gameObject.tag == "Bullet")
        {
            if (other.gameObject.tag == "Bullet")
            {
                Destroy(other.gameObject);
            }
            Life--;
            if (Life == 0)
            {
                Destroy(gameObject);
            }
            else
            {
                StartRecovery(1);
            }
        }
    }

    public void Impact(Vector3 force)
    {
        Vector3 dir = force.normalized;
        dir.y = 0.5f;
        impact += dir.normalized * force.magnitude / mass;
    }
}
