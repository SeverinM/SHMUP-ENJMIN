using System;
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
    protected float life = 3;

    internal GameObject protection;

    [SerializeField]
    [Header("Sci-fi magnetic protection")]
    internal GameObject protectionPrefab;

    public Level level;

    public GameObject leader;

    internal void SetLeader(GameObject leader)
    {
        this.leader = leader;
    }

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
            life = value;
            if (life <= 0)
            {
                // On demande au niveau de retirer le character du niveau
                level.Remove(this);
            }
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
        transform.Translate(new Vector3(movement.x * Time.deltaTime * moveSpeed * PersonalScale * GetScale(), 0, movement.y * Time.deltaTime * moveSpeed * PersonalScale * GetScale()), Space.World);
    }

    public virtual void Move(Vector3 movement)
    {
        transform.Translate(movement * Time.deltaTime * moveSpeed * PersonalScale * GetScale(), Space.World);
    }

    public void StartRecovery(float duration)
    {
        SetState(new CharacterRecovery(this, duration));
    }

    //Coroutine generique permettant d'effectuer une action en decalé
    public void StartDelayedTask(UnityAction before , float duration, UnityAction after)
    {
        duration = Mathf.Max(Mathf.Abs(duration), 0.1f);
        StartCoroutine(DelayedActionCoroutine(before, duration, after));
    }

    IEnumerator DelayedActionCoroutine(UnityAction before , float duration , UnityAction after)
    {
        before();
        yield return new WaitForSeconds(duration);
        after();
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

    //Permet de separer les ennemies entre eux pour eviter qu'ils se marchent dessus
    public void Separate()
    {
        float desiredseparation = 6f;
        List<GameObject> characters = new List<GameObject>();
        foreach (RaycastHit hit in Physics.SphereCastAll(transform.position, desiredseparation, Vector3.zero))
        {
            if (hit.collider.GetComponent<Enemy>() != null)
            {
                characters.Add(hit.collider.gameObject);
            }
        }

        float maxForce = 2f;
        Vector3 sum = new Vector3();
        int count = 0;

        foreach (GameObject other in characters)
        {
            if (other.Equals(this)) break;

            float d = Vector3.Distance(transform.position, other.transform.position);
            Vector3 diff = transform.position - other.transform.position;
            diff.Normalize();
            diff /= d;
            sum += diff;
            count++;
        }

        if (count > 0)
        {
            sum /= count;
            sum.Normalize();
            sum *= 5f;
            Vector3 steer = sum - GetComponent<Rigidbody>().velocity;
            Vector3.ClampMagnitude(steer, maxForce);
            GetComponent<Rigidbody>().AddForce(steer);
        }
    }
}
