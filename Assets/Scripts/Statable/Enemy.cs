using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;

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
        foreach (WaypointElement we in allWaypoints)
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

public class Enemy : Character
{

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
            default:
                FollowRandomPath();
                break;
        }

        protection = Instantiate(protectionPrefab, transform);
        protection.SetActive(false);
    }

    public void SetWaypointsAndApply(Waypoints value)
    {
        Waypoints = value;
        FollowPath();
    }

    /// <summary>
    /// Le personnage suit un chemin défini par un GD/Ergonome de qualité
    /// </summary>
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
        int stepAngle = 10;
        waypoints.loop = false;
        Waypoints.allWaypoints.Clear();
        int movements = Random.Range(2, 5);
        float CurrentAngle = Random.Range(0, Mathf.PI * 2);
        //Projection de la position du personage apres X waypoints
        Vector3 potentialPosition = transform.position;

        for (int i = 0; i < movements; i++)
        {
            WaypointElement wE = new WaypointElement();
            //Si la direction est hors ecran on trouve une autre direction
            for (int nbTry = 1; nbTry < stepAngle; nbTry++)
            {
                float modifiedAngle = (CurrentAngle + (((Mathf.PI * 2) / stepAngle) * nbTry)) % Mathf.PI * 2;
                Vector3 direction = new Vector3(Mathf.Cos(modifiedAngle), 0, Mathf.Sin(modifiedAngle)) * 4;
                //La nouvelle position est valide , on quitte la boucle et on applique la nouvelle position imaginaire
                if (Utils.IsInCamera(potentialPosition + direction, Mathf.Abs(transform.position.y - Camera.main.transform.position.y)))
                {
                    potentialPosition += direction;
                    break;
                }
            }

            wE.speed = MoveSpeed;
            wE.targetPosition = potentialPosition;
            Waypoints.allWaypoints.Add(wE);
        }

        Queue<WaypointElement> allPos = new Queue<WaypointElement>(Waypoints.allWaypoints);
        SetState(new FollowPathMovement(this, allPos, true));
    }

    /// <summary>
    /// Le personnage suit un autre personnage
    /// </summary>
    private void FollowCharacter()
    {
        if (Leader != null)
            SetState(new EnemyMovement(this, Leader.transform, new Queue<WaypointElement>(), true));
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

    public override float GetScale()
    {
        return Constants.TimeScaleEnnemies;
    }
}
