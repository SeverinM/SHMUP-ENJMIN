using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

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
        Normal,
        NormalDash,
    }

    [Header("Mouvement")]
    [SerializeField]
    [Tooltip("Longueur d'un dash")]
    float distanceDash = 2;
    public float DistanceDash
    {
        get
        {
            return distanceDash;
        }
    }
    [SerializeField]
    [Tooltip("Temps de régeneration des dash")]
    internal float dashCooldown = 2f;

    [SerializeField]
    [Tooltip("Maximum dashes")]
    internal int maxDashes = 3;

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

    [Header("Vitesse dash")]
    [Tooltip("A quel point la vitesse est reduite par rapport a la vitesse normale ? (exemple : 0.1 signifie 10 fois moins vite)")]
    [SerializeField]
    internal float dashSpeed = 5f;

    [Header("Auto references (pas toucher... normalement)")]
    [SerializeField]
    Transform barrier;
    public Transform Barrier
    {
        get
        {
            return barrier;
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

    internal LineRenderer line;

    internal Transform target;
    public Transform Target
    {
        get
        {
            return target;
        }
    }

    internal Vector3 origin;

    public delegate void voidParam();
    public event voidParam NextLevel;

    public void RaiseNextLevel()
    { 
        if (NextLevel != null)
            NextLevel();
    }

    void Start()
    {
        context.SetInDictionary("Mode", mode);
        context.SetInDictionary("Hook", hook);
        context.SetInDictionary("Barrier", barrier);
        context.SetInDictionary("SpeedWinch", speedPull);
        context.SetInDictionary("SpeedHook", hookSpeed);
        context.SetInDictionary("RangeDash", distanceDash);
        context.SetInDictionary("CoeffHook", coeffHook);
        context.SetInDictionary("RangeHook", rangeHook);

        line = hook.GetComponent<LineRenderer>();
        origin = hook.transform.localPosition;
        ResetHook();

        protection = Instantiate(protectionPrefab, transform);
        protection.SetActive(false);

        actualState = new PlayerMovement(this);
    }

    new void Update()
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

        // Definir la position de la ligne uniquement que si il y a un target
        line.SetPosition(0, transform.position);

        if (target != null)
            line.SetPosition(1, target.position);
        else
            line.SetPosition(1, transform.position);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Quand le joueur se fait toucher par un rigidBody
        
        //En suspens pour le moment
        if (collision.gameObject.tag == "Bullet")
        {
            Impact(collision.relativeVelocity * hitForce);
            Destroy(collision.gameObject);

            if (!base.protection.activeInHierarchy)
            {
                Life--;

                if (Life <= 0)
                {
                    Destroy(gameObject);
                }
                else
                {
                    StartRecovery(recoveryDuration);
                    AkSoundEngine.PostEvent("S_Hurt", gameObject);
                }
            }
        }
    }

    // Le joueur se voit propulsé dans la direction opposée à un impact reçu
    public void Impact(Vector3 force)
    {
        Vector3 dir = force.normalized;
        dir.y = 0; // En hauteur
        impact += dir.normalized * force.magnitude / mass;
    }

    public override float GetScale()
    {
        return Constants.TimeScalePlayer;
    }

    /// <summary>
    /// Mettre a jour la position du hook
    /// </summary>
    public void UpdateHook()
    {
        hook.transform.localPosition = origin;
        target = hook.transform;
        hook.forward = transform.forward;
    }

    /// <summary>
    /// Reinitialiser la position du hook
    /// </summary>
    public void ResetHook()
    {
        hook.transform.localPosition = origin;
        target = hook.transform;
    }

    /// <summary>
    /// Definir l'entitée a laquelle Hook est attaché
    /// </summary>
    /// <param name="transform"></param>
    public void AttachHook(Transform transform)
    {
        target = transform;
    }

    public override void OnDestroy()
    {
        if (!Constants.ApplicationQuit)
        {
            base.OnDestroy();
            //SetState(null);
            //Utils.StartFading(1, Color.black, () => { Constants.SetAllConstants(0); SceneManager.LoadScene(SceneManager.GetActiveScene().name); }, () => Constants.SetAllConstants(1));
            Constants.ApplicationQuit = true;
            Manager.GetInstance().PopAll();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
