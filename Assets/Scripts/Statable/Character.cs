using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public float spawnAfter;
    public Waypoints Waypoints;
    //Cet attribut est utilisé uniquement dans l'outil
    [HideInInspector]
    public bool selected;
    [HideInInspector]
    public bool spawned;

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

    [Header("Herité de Character")]
    [Tooltip("A quel vitesse le personnage peut se deplacer ?")]
    [SerializeField]
    //Speed of the character while moving
    protected float moveSpeed = 6.0f;

    [SerializeField]
    protected float mass = 3.0f;                
    protected float hitForce = 25.5f;            
    protected Vector3 impact = Vector3.zero;
    protected float recoveryDuration = 1f;
    protected float freezeDuration = 1f;

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
    protected float life = 3;

    [SerializeField]
    [Header("Sci-fi magnetic protection")]
    internal GameObject protection;

    internal void Rotate(GameObject player)
    {
        transform.LookAt(player.transform);
    }

    public float Life
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
        transform.Translate(new Vector3(movement.x * Time.deltaTime * moveSpeed, 0, movement.y * Time.deltaTime * moveSpeed), Space.World);
    }

    public virtual void Move(Vector3 movement)
    {
        transform.Translate(new Vector3(movement.x * Time.deltaTime * moveSpeed, movement.y * Time.deltaTime * moveSpeed, movement.z * Time.deltaTime * moveSpeed), Space.World);
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


}
