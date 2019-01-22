using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Wave
{
    public List<WaveElement> allEnnemies;
    public bool healAtBegin;
    public float delay;
    public bool firstIsLeader;
    public float spacingX;
    public float spacingZ;
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
    public float speed;
    public int life;
    [HideInInspector]
    public bool spawned;
    public bool followPlayer;
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

    public delegate void charaParam(Character chara);
    public event charaParam Destroyed;

    public delegate Player PlayerDelagate();
    public event PlayerDelagate TryReachingPlayer;

    [Header("Herité de Character")]
    [Tooltip("A quel vitesse le personnage peut se deplacer ?")]
    [SerializeField]
    protected float moveSpeed = 6.0f;

    public float MoveSpeed
    {
        get
        {
            return moveSpeed;
        }
        set
        {
            moveSpeed = Mathf.Abs(value);

        }
    }

    [SerializeField]
    [Tooltip("Plus il est grand plus il prendra de temps a se tourner , est proportionnel avec la vitesse")]
    protected float coeffRotation = 1;
    public float CoeffRotation
    {
        get
        {
            return coeffRotation;
        }
    }

    [SerializeField]
    [Tooltip("Qui doit on utiliser pour changer le material en cas de recovery ?")]
    GameObject model;
    public GameObject Model
    {
        get
        {
            return model;
        }
    }

    [SerializeField]
    protected float mass = 3.0f;                
    protected float hitForce = 25.5f;            
    protected Vector3 impact = Vector3.zero;
    //Combien de temps le joueur est invincible une fois touché ?

    [SerializeField]
    protected float recoveryDuration = 2f;
    protected float freezeDuration = 1f;
    internal float scaleDuration = 1f;
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

    protected Binding<int> watchedLife;
    public virtual int Life
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


    [SerializeField]
    [Tooltip("Nombre de dash")]
    protected int dash = 3;

    internal GameObject protection;

    [SerializeField]
    [Header("Sci-fi magnetic protection")]
    internal GameObject protectionPrefab;

    [Header("Bullet")]
    [SerializeField]
    [Tooltip("Déceleration lors d'un impact de bullet")]
    protected float impactDeceleration = 5f;

    GameObject leader;
    public GameObject Leader
    {
        get
        {
            return leader;
        }

        set
        {
            leader = value;
            if (leader != null)
                SetState(new EnemyMovement(this, leader.transform,true));
        }
    }

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

    internal void Rotate(GameObject player)
    {
        transform.LookAt(player.transform);
        Quaternion.Euler(transform.rotation.x, transform.rotation.y, 0);
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

    public void NextState()
    {
        if (ActualState != null)
            ActualState.NextState();
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

        // Déplacer le personnage lors d'un impact de rigidBody
        if (impact.magnitude > 0.2)
        {
            Move(impact * Time.deltaTime);
        }

        // Impact tend vers zero
        impact = Vector3.Lerp(impact, Vector3.zero, impactDeceleration * Time.deltaTime);
    }

    public virtual void Hit(Vector3 speed)
    {
        Impact(speed);
        Life--;
    }

    // Le joueur se voit propulsé dans la direction opposée à un impact reçu
    public void Impact(Vector3 force)
    {
        Vector3 dir = force.normalized;
        dir.y = 0;
        impact += dir.normalized * force.magnitude / mass;
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
        if (!Context.ValuesOrDefault<bool>("InRecovery",false))
        {
            StartCoroutine(StartRecoveryCoroutine(duration));
        }
            
    }

    public IEnumerator StartRecoveryCoroutine(float duration)
    {
        Context.SetInDictionary("InRecovery", true);
        float timeBegin = 0;
        Dictionary<Transform, Material> allMats = new Dictionary<Transform, Material>();
        allMats[model.transform] = model.GetComponent<MeshRenderer>().material;
        foreach(Transform trsf in model.GetComponentsInChildren<Transform>())
        {
            if (trsf.GetComponent<MeshRenderer>() != null)
            {
                allMats[trsf] = trsf.GetComponent<MeshRenderer>().material;
                if (recoveryMat != null)
                {
                    trsf.GetComponent<MeshRenderer>().material = recoveryMat;
                }
            }          
        }

        GetComponent<Collider>().enabled = false;

        while(timeBegin < duration)
        {
            timeBegin += Time.deltaTime * GetScale() * PersonalScale;
            yield return null;
        }

        foreach(Transform key in allMats.Keys)
        {
            if (key != null)
                key.GetComponent<MeshRenderer>().material = allMats[key];
        }
        GetComponent<Collider>().enabled = true;
        Context.Remove("InRecovery");
    }


    /// <summary>
    /// Chaque sous-classe a son propre timeScale , utilisez de preference un attribut facilement accessible
    /// </summary>
    /// <returns></returns>
    public abstract float GetScale();

    public virtual void OnDestroy()
    {
        if (!Constants.ApplicationQuit)
        {
            if (Destroyed != null)
                Destroyed(this);
        }
    }

    public Vector3 Separate(Vector3 initialDirection, float desiredseparation = 6)
    {
        Vector3 output = initialDirection.normalized;
        foreach (RaycastHit hit in Physics.SphereCastAll(transform.position, desiredseparation, Vector3.forward))
        {
            if (hit.collider.tag == "Ennemy" && hit.collider.transform != transform)
            {
                //Plus la cible est loin et moins on s'ecarte
                Vector3 deltaPosition = (transform.position - hit.transform.position);
                //S'il se trouve exactement sur la meme position , en trouve une aleatoire
                deltaPosition.Normalize();
                if (deltaPosition == Vector3.zero)
                {
                    Vector2 temp = UnityEngine.Random.insideUnitCircle;
                    temp.Normalize();
                    deltaPosition = new Vector3(temp.x, 0, temp.y);
                }

                //Si on sort de la camera en faisant ca , annule le mouvement
                if (!Utils.IsInCamera(transform.position + deltaPosition, Mathf.Abs(transform.position.y - Camera.main.transform.position.y)))
                {
                    deltaPosition = Vector3.zero;
                }

                float coeff = Mathf.Max(1, Vector3.Distance(hit.transform.position, transform.position));
                output += deltaPosition * (coeff / desiredseparation);
            }
        }
        output.Normalize();
        return output;
    }

    //Permet de changer d'etat en differé
    public void StartDelayedState(float duration, State st)
    {
        StartCoroutine(DelayedSetState(duration, st));
    }

    public IEnumerator DelayedSetState(float duration, State st)
    {
        yield return new WaitForSeconds(duration);
        SetState(st);

    }

    private void OnApplicationQuit()
    {
        Constants.ApplicationQuit = true;
    }
}
