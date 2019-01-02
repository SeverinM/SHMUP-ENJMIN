using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class Waypoints
{
    public bool loop;
    public List<WaypointElement> allWaypoints;

    public Waypoints Clone()
    {
        Waypoints wp = new Waypoints();
        wp.loop = loop;
        wp.allWaypoints = new List<WaypointElement>();
        foreach(WaypointElement we in allWaypoints)
        {
            wp.allWaypoints.Add(we.Clone());
        }
        return wp;
    }
}

[System.Serializable]
public class WaypointElement
{
    public Vector3 targetPosition;
    public float speed;

    public WaypointElement Clone()
    {
        WaypointElement output = new WaypointElement();
        output.targetPosition = targetPosition;
        output.speed = speed;
        return output;
    }
}

public class Enemy : Character {

    public enum EnemyMovementType
    {
        FOLLOW_GAME_OBJECT,
        FOLLOW_PATH,
        FOLLOW_RANDOM_PATH
    }

    public enum EnemyType
    {
        BOB,
        JIM,
        MIKE
    }

    [SerializeField]
    protected int HP = 0;

    [SerializeField]
    //Si la distance ennemi / joueur est inferieur a cette distance , l'ennemi va attaquer le joueur au lieu de poursuivre
    protected float attackRange = 2.0f;
    public float AttackRange
    {
        get
        {
            return attackRange;
        }
    }


    [SerializeField]
    protected float shootPeriod = 2.0f;
    public float ShootPeriod
    {
        get
        {
            return shootPeriod;
        }
    }

    [SerializeField]
    protected int shootAmount = 3;
    public int ShootAmount
    {
        get
        {
            return shootAmount;
        }
    }

    [SerializeField]
    //A quel vitesse les balles progressent
    protected float shootSpeed = 8;
    public float ShootSpeed
    {
        get
        {
            return shootSpeed;
        }
    }

    [SerializeField]
    protected int maxBullets = 5;
    public float MaxBullets
    {
        get
        {
            return maxBullets;
        }
    }

    [SerializeField]
    protected float shootRadius = 1f;

    public float ShootRadius
    {
        get
        {
            return shootRadius;
        }
    }

    Waypoints waypoints;
    public Waypoints Waypoints
    {
        get
        {
            return waypoints;
        }

        set
        {
            waypoints = value;
        }
    }

    [SerializeField]
    private GameObject bulletPrefab;

    [SerializeField]
    internal EnemyMovementType movementType;

    [SerializeField]
    public EnemyType enemyType;

    private void Start()
    {
        Context.SetInDictionary("Level", level);
        switch (movementType)
        {
            case EnemyMovementType.FOLLOW_GAME_OBJECT:
                FollowCharacter();
                break;
            case EnemyMovementType.FOLLOW_PATH:
                FollowPath();
                break;
            case EnemyMovementType.FOLLOW_RANDOM_PATH:
                FollowRandomPath();
                break;
        }

        protection = Instantiate(protectionPrefab, transform);
    }

    public void SetWaypointsAndApply(Waypoints value)
    {
        Waypoints = value;
        FollowPath();
    }

    public void FollowPath()
    {
        //Toutes les positions globales
        if (Waypoints.allWaypoints != null)
        {
            Queue<WaypointElement> allPos = new Queue<WaypointElement>(Waypoints.allWaypoints);
            SetState(new FollowPathMovement(this, allPos, Waypoints.loop));
        }
    }

    /// <summary>
    /// Le personnage suit des positions choisies aléatoirement
    /// </summary>
    public void FollowRandomPath()
    {
        SetState(new EnemyMovement(this, new Queue<WaypointElement>()));
    }

    //Chaque ennemi agit differemment selon son type
    public void Shoot()
    {
        Rigidbody clone;
        switch (enemyType)
        {
            case EnemyType.BOB:
                //Tir simple en face de lui
                clone = Instantiate(bulletPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.rotation).GetComponent<Rigidbody>();
                clone.velocity = transform.forward * shootSpeed;
                Debug.Log(clone.velocity.magnitude);
                break;
            case EnemyType.JIM:
                float x = 0, y = 0;

                //L'angle du premier projectile est aleatoire
                float angle = UnityEngine.Random.Range(0, 2 * Mathf.PI);

                //Tir en rayon a angle uniforme
                for (int i = 0; i < maxBullets; i++)
                {
                    x = (shootRadius * Mathf.Cos(angle)) + transform.position.x;
                    y = (shootRadius * Mathf.Sin(angle)) + transform.position.z;

                    angle += ((2 * Mathf.PI) / maxBullets);
                    angle = angle % (2 * Mathf.PI);
                    clone = Instantiate(bulletPrefab, new Vector3(x, 0, y), Quaternion.AngleAxis(angle * Mathf.Rad2Deg, new Vector3(0, 1, 0))).GetComponent<Rigidbody>();
                    Vector3 direction = Quaternion.Euler(0, angle, 0) * clone.transform.forward;
                    clone.velocity = transform.TransformDirection(direction * shootSpeed);
                }
                break;
            case EnemyType.MIKE:

                break;
        }

    }

    private void FollowCharacter()
    {
        SetState(new EnemyMovement(this, new Queue<WaypointElement>()));
    }

    public override float GetScale()
    {
        return Constants.TimeScaleEnnemies;
    }
}
