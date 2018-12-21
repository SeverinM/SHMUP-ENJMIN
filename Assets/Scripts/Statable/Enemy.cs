using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public float range = 2.0f;
    public float shootPeriod = 2.0f;
    public int shootAmount = 3;
    public float shootSpeed = 8;
    public int maxBullets = 5;
    public float shootRadius = 1f;

    public GameObject player;
    public Level level;

    [SerializeField]
    private GameObject bulletPrefab;

    [SerializeField]
    private EnemyMovementType movementType;

    [SerializeField]
    private EnemyType enemyType;

    private void Start()
    {
        switch (movementType)
        {
            case EnemyMovementType.FOLLOW_GAME_OBJECT:
                FollowGameObject();
                break;
            case EnemyMovementType.FOLLOW_PATH:
                FollowPath();
                break;
            case EnemyMovementType.FOLLOW_RANDOM_PATH:
                FollowRandomPath();
                break;
        }
    }

    public void StartFreeze()
    {
        StartCoroutine(Freeze(0.1f));
    }

    /// <summary>
    /// L'ennemi a subit des degats
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
    IEnumerator Freeze(float duration = 1)
    {
        Life -= 1;
        if (Life > 0)
        {
            StartRecovery(duration * 10);
        }

        else
        {
            GetComponent<Collider>().enabled = false;
            GetComponent<MeshRenderer>().enabled = false;
            Constants.TimeScalePlayer = 0;
            yield return new WaitForSeconds(duration);
            Constants.TimeScalePlayer = 1;
            Destroy(gameObject);
        }
    }

    private void FollowPath()
    {
        Queue<Vector3> allPos = new Queue<Vector3>();
        allPos.Enqueue(new Vector3(0, 0, -5));
        allPos.Enqueue(new Vector3(-2, 0, -2));
        allPos.Enqueue(new Vector3(2, 0, -2));

        SetState(new FollowPathMovement(this, level, allPos, false));
    }

    private void FollowRandomPath()
    {
        SetState(new EnemyMovement(this, level, player.transform));
    }

    public void Shoot()
    {
        Rigidbody clone;
        switch (enemyType)
        {
            case EnemyType.BOB:
                clone = Instantiate(bulletPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.rotation).GetComponent<Rigidbody>();
                clone.velocity = transform.TransformDirection(Vector3.forward * shootSpeed);
                break;
            case EnemyType.JIM:
                float x = 0, y = 0, angle = 0;

                for (int i = 0; i < maxBullets; i++)
                {
                    x = (shootRadius * Mathf.Cos(angle)) + transform.position.x;
                    y = (shootRadius * Mathf.Sin(angle)) + transform.position.z;

                    angle = angle + ((2 * Mathf.PI) / maxBullets);
                    clone = Instantiate(bulletPrefab, new Vector3(x, 0, y), Quaternion.AngleAxis(angle * Mathf.Rad2Deg, new Vector3(0, 1, 0))).GetComponent<Rigidbody>();
                    Vector3 direction = Quaternion.Euler(0, angle, 0) * clone.transform.forward;
                    clone.velocity = transform.TransformDirection(direction * shootSpeed);
                }
                break;
            case EnemyType.MIKE:

                break;
        }

    }

    private void FollowGameObject()
    {
        SetState(new EnemyMovement(this, level, player.transform));
    }

    public override float GetScale()
    {
        return Constants.TimeScaleEnnemies;
    }
}
