﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Un objet character PEUT avoir des etats , agissant comme une machine d'etat
/// </summary>
public abstract class Character : MonoBehaviour {

    public delegate void collDelegate(Collider coll);
    public event collDelegate OnTriggerEnterChar;

    [Header("Herité de Character")]
    [Tooltip("A quel vitesse le personnage peut se deplacer ?")]
    [SerializeField]
    //Speed of the character while moving
    protected float moveSpeed = 6.0f;

    [SerializeField]
    protected float mass = 3.0f;                
    protected float hitForce = 25.5f;            
    protected Vector3 impact = Vector3.zero;

    [SerializeField]
    [Tooltip("Material de substitution pendant que le personnage est en recovery (ATTENTION : peut bugger s'il y a plusieurs materials)")]
    Material recoveryMat;

    [SerializeField]
    [Tooltip("Nombre de point de vie du personnage , un nombre negatif equivaut a 1")]
    protected int life = 3;
    public int Life
    {
        get
        {
            return life;
        }

        set
        {
            life = Mathf.Abs(value);
        }
    }

    [SerializeField]
    protected State actualState;
    public State ActualState
    {
        get
        {
            return actualState;
        }
    }

    public void SetState(State state)
    {
        if (actualState != null)
        {
            actualState.EndState();
        }
        actualState = state;

        if (actualState != null)
        {
            actualState.StartState();
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (OnTriggerEnterChar != null)
            OnTriggerEnterChar(other);
    }

    protected void Update()
    {
        if (actualState != null)
        {
            actualState.UpdateState();
        }
    }

    public void InterpretInput(BaseInput.TypeAction typAct, BaseInput.Actions baseInput, Vector2 value)
    {
        if (actualState != null)
        {
            actualState.InterpretInput(typAct, baseInput, value);
        }
    }

    public virtual void Move(Vector2 movement)
    {
        transform.Translate(new Vector3(movement.x * Time.deltaTime * moveSpeed, 0, movement.y * Time.deltaTime * moveSpeed), Space.World);
    }

    public virtual void Move(Vector3 movement)
    {
        transform.Translate(new Vector3(movement.x * Time.deltaTime * moveSpeed, movement.y * Time.deltaTime * moveSpeed, movement.z * Time.deltaTime * moveSpeed), Space.World);
    }

    public void StartRecovery(float duration)
    {
        StartCoroutine(Recovery(duration));
    }

    /// <summary>
    /// Pendant un cours instant , change l'aspect du personnage et desactive sa physique
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
    protected IEnumerator Recovery(float duration)
    {
        Collider physic = GetComponent<Collider>();
        Material copy = GetComponent<MeshRenderer>().material;
        if (recoveryMat != null)
        {
            GetComponent<MeshRenderer>().material = recoveryMat;
        }
        physic.enabled = false;
        yield return new WaitForSeconds(duration);
        physic.enabled = true;
        GetComponent<MeshRenderer>().material = copy;
    }
}
