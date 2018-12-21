using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Player : Character
{
    [Header("Debug")]
    [SerializeField]
    MovementMode mode;
    public MovementMode Mode
    {
        get
        {
            return mode;
        }
    }

    public enum MovementMode
    {
        Dash,
        Normal
    }

    [Header("Mouvement")]
    [SerializeField]
    [Tooltip("Longueur d'un dash")]
    float distanceDash = 2;

    [Header("Tir du grappin")]
    [SerializeField]
    [Tooltip("A quel vitesse progresse le grappin")]
    float hookSpeed = 0.7f;

    [SerializeField]
    [Tooltip("Portée du hook")]
    float rangeHook = 10;

    [Header("Hook / Winch")]
    [Tooltip("Vitesse du pull / winch")]
    [SerializeField]
    float speedPull = 10;

    [Header("Mouvement durant le hook")]
    [Tooltip("A quel point la vitesse est reduite par rapport a la vitesse normale ? (exemple : 0.1 signifie 10 fois moins vite)")]
    [SerializeField]
    float coeffHook = 0.1f;

    [Header("Auto references (pas toucher... normalement)")]
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
        context.SetInDictionary("Mode", mode);
        context.SetInDictionary("Hook", hook);
        context.SetInDictionary("Shield", shield);
        context.SetInDictionary("SpeedWinch", speedPull);
        context.SetInDictionary("SpeedHook", hookSpeed);
        context.SetInDictionary("RangeDash", distanceDash);
        context.SetInDictionary("CoeffHook", coeffHook);
        context.SetInDictionary("RangeDash", rangeHook);
        actualState = new PlayerMovement(this);
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
        //Le joueur s'est fait touché par un ennemi
        if (other.GetComponent<Enemy>() != null || other.gameObject.tag == "Bullet")
        {
            if (other.gameObject.tag == "Bullet")
            {
                Destroy(other.gameObject);
            }
            Life--;
            if (Life <= 0)
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

    private void OnDestroy()
    {
        Debug.Log("Pouf plus de joueur");
    }

    public override float GetScale()
    {
        return Constants.TimeScalePlayer;
    }
}
