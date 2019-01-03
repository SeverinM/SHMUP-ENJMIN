﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Wave
{
    public List<WaveElement> allEnnemies;
    public float delay;
}

[System.Serializable]
public class WaveElement
{
    public Enemy.EnemyType enn;
    public Enemy.EnemyMovementType enMov;
    public float spawnAfter;
    public Waypoints Waypoints;

    //Cet attribut est utilisé uniquement dans l'outil
    [HideInInspector]
    public bool selected;
    [HideInInspector]
    public bool spawned;

    public bool isLeader;

    public void End()
    {
        spawned = true;
    }
}
/// <summary>
/// Un objet character PEUT avoir des etats , agissant comme une machine d'etat
/// </summary>
public abstract class Character : MonoBehaviour {

    public delegate void collDelegate(Collider coll);
    public event collDelegate OnTriggerEnterChar;
    public event collDelegate OnTriggerExitChar;

    public delegate void voidParam(Character chara);
    public event voidParam Destroyed;

    public delegate Player PlayerDelagate();
    public event PlayerDelagate TryReachingPlayer;

    [Header("Herité de Character")]
    [Tooltip("A quel vitesse le personnage peut se deplacer ?")]
    [SerializeField]
    protected float moveSpeed = 6.0f;

    [SerializeField]
    protected float mass = 3.0f;                
    protected float hitForce = 25.5f;            
    protected Vector3 impact = Vector3.zero;
    //Combien de temps le joueur est invincible une fois touché ?
    protected float recoveryDuration = 1f;
    protected float freezeDuration = 1f;
    protected float personalScale = 1f;
    public float PersonalScale
    {
        get
        {
            return personalScale;
        }
        set
        {
            personalScale = Mathf.Abs(value);
        }
    }

    [SerializeField]
    [Tooltip("Material de substitution pendant que le personnage est en recovery (ATTENTION : peut bugger s'il y a plusieurs materials)")]
    internal Material recoveryMat;

    /// <summary>
    /// Sert a conserver des informations generiques entre les etats quand leur nombre devient important
    /// </summary>
    protected Context context = new Context();
    public Context Context
    {
        get
        {
            return context;
        }
    }

    [SerializeField]
    [Tooltip("Nombre de point de vie du personnage , un nombre negatif equivaut a 1")]
    protected int life = 3;

    Binding<int> watchedLife;
    public int Life
    {
        get
        {
            return watchedLife.WatchedValue;
        }

        set
        {
            watchedLife.WatchedValue = value;
            if (watchedLife.WatchedValue <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    internal GameObject protection;

    [SerializeField]
    [Header("Sci-fi magnetic protection")]
    internal GameObject protectionPrefab;

    public GameObject leader;

    void Awake()
    {
        watchedLife = new Binding<int>(0, null);
    }

    //Appellé a chaque fois que la vie du joueur change
    public void SetOnLifeChanged(Action<int> newAction)
    {
        watchedLife.ValueChanged = newAction;
        //Reactualise avec la valeur actuelle
        Life = life;
    }

    internal void SetLeader(GameObject leader)
    {
        this.leader = leader;
    }

    internal void Rotate(GameObject player)
    {
        transform.LookAt(player.transform);
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

    //On ne fait qu'envoyer un event , c'est le state qui decide comment gerer cet evenement
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (OnTriggerEnterChar != null)
            OnTriggerEnterChar(other);
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (OnTriggerExitChar != null)
            OnTriggerExitChar(other);
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
        transform.Translate(new Vector3(movement.x * Time.deltaTime * moveSpeed * PersonalScale * GetScale(), 0, movement.y * Time.deltaTime * moveSpeed * PersonalScale * GetScale()), Space.World);
    }

    public virtual void Move(Vector3 movement)
    {
        transform.Translate(movement * Time.deltaTime * moveSpeed * PersonalScale * GetScale(), Space.World);
    }

    public void StartRecovery(float duration)
    {
        SetState(new CharacterRecovery(this, actualState, duration));
    }

    /// <summary>
    /// Chaque sous-classe a son propre timeScale , utilisez de preference un attribut facilement accessible
    /// </summary>
    /// <returns></returns>
    public abstract float GetScale();

    public void OnDestroy()
    {
        if (Destroyed != null)
        {
            Destroyed(this);
        }
    }

    /// <summary>
    /// Retourne un joueur dont des valeurs ont été changées 
    /// </summary>
    /// <returns></returns>
    public Player RaiseTryReaching()
    {
        Player output = null;
        if (TryReachingPlayer != null)
        {
            output = TryReachingPlayer();
        }
        return output;
    }
}
