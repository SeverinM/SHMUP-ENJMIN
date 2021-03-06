using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;


public class Player : Character
{
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
    [Tooltip("Temps de regeneration des dash")]
    internal float dashCooldown = 2f;

    [SerializeField]
    [Tooltip("Maximum dashes")]
    internal int maxDashes = 3;

    [Header("Tir du grappin")]
    [SerializeField]
    [Tooltip("A quel vitesse progresse le grappin")]
    float hookSpeed = 0.7f;

    [SerializeField]
    [Tooltip("Port�e du hook")]
    float rangeHook = 10;

    [Header("Hook / Winch")]
    [Tooltip("Vitesse du pull / winch")]
    [SerializeField]
    float speedPull = 10;

    [Header("ScreenShake lors d'un impact")]
    [Tooltip("Force lors d'un impact")]
    [SerializeField]
    float screenShakeForce = 0.8f;
    public float ScreenShakeForce
    {
        get
        {
            return screenShakeForce;
        }
    }

    [Tooltip("Duree lors d'un impact")]
    [SerializeField]
    float screenShakeDuration = 0.011f;
    public float ScreenShakeDuration
    {
        get
        {
            return screenShakeDuration;
        }
    }

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
    public event voidParam BlackScreen;

    public void RaiseNextLevel()
    { 
        if (NextLevel != null)
            NextLevel();
    }

    public void RaiseBlackScreen()
    {
        if (BlackScreen != null)
            BlackScreen();
    }

    void Start()
    {
        context.SetInDictionary(Constants.HOOK, hook);
        context.SetInDictionary(Constants.BARRIER, barrier);
        context.SetInDictionary(Constants.SPEED_WINCH, speedPull);
        context.SetInDictionary(Constants.SPEED_HOOK, hookSpeed);
        context.SetInDictionary(Constants.RANGE_DASH, distanceDash);
        context.SetInDictionary(Constants.COEFF_HOOK, coeffHook);
        context.SetInDictionary(Constants.RANGE_HOOK, rangeHook);

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

        // D�placer le personnage lors d'un impact de rigidBody
        if (impact.magnitude > 0.2 && Utils.IsInCamera(transform.position + impact, Mathf.Abs(transform.position.y - Camera.main.transform.position.y)))
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
       
        if (collision.gameObject.tag == Constants.BULLET_TAG)
        {
            //On ne se subit rien si le joueur est en recovery ou en train de winch
            if (!Context.ValuesOrDefault<bool>(Constants.IN_RECOVERY, false) && !GetComponentInChildren<Barrier>().IsWinching)
                Hit(collision.relativeVelocity * hitForce); 
            
            Destroy(collision.gameObject);                       
        }
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
    /// Definir l'entit�e a laquelle Hook est attach�
    /// </summary>
    /// <param name="transform"></param>
    public void AttachHook(Transform transform)
    {
        target = transform;
    }

    public override void Hit(Vector3 speed)
    {
        Impact(speed);

        Manager.GetInstance().ShakeCamera(screenShakeForce, screenShakeDuration);

        if (Life <= 1)
        {
            AkSoundEngine.PostEvent("S_Destroy", gameObject);
        }
        else
        {
            StartRecovery(recoveryDuration);
            AkSoundEngine.PostEvent("S_Hurt", gameObject);
        }

        Life--;
    }

    public override void OnDestroy()
    {
        if (!Constants.ApplicationQuit)
        {
            base.OnDestroy();
            Manager.GetInstance().PostScore();
            //A sa mort reset le score du joueur
            Constants.ApplicationQuit = true;
            Manager.GetInstance().PopAll();
            Utils.StartFading(1, Color.black, () => { Constants.SetAllConstants(0); SceneManager.LoadScene(SceneManager.GetActiveScene().name); }, () => { Constants.SetAllConstants(1); Constants.TotalScore = 0;});
        }
    }

}
