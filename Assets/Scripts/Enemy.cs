﻿using System;
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

    [SerializeField]
    protected int HP = 0;

    public float range = 2.0f;
    public float shootPeriod = 2.0f;
    public int shootAmount = 3;
    public int maxBullets = 5;
    public float shootRadius = 1f;

    public GameObject player;
    public Level level;

    [SerializeField]
    private GameObject bulletPrefab;

    [SerializeField]
    private EnemyMovementType movementType;


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

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Shield")
        {
            StartCoroutine(Freeze(0.1f));
        }

        if (other.tag == "Hook")
        {
            SetState(null);
        }
    }

    IEnumerator Freeze(float duration = 1)
    {
        GetComponent<MeshRenderer>().enabled = false;
        Constants.TimeScalePlayer = 0;
        yield return new WaitForSeconds(duration);
        Constants.TimeScalePlayer = 1;
        level.Remove(gameObject);
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
        SetState(new MovementEnemy(this, level, player.transform));
    }

    public void Shoot()
    {
        float x = 0, y = 0, angle = 0;

        for (int i = 0; i < maxBullets; i++)
        {
            x = (shootRadius * Mathf.Cos(angle)) + transform.position.x;
            y = (shootRadius * Mathf.Sin(angle)) + transform.position.z;

            angle = angle + ((2 * Mathf.PI) / maxBullets);
            Instantiate(bulletPrefab, new Vector3(x, 0, y), Quaternion.AngleAxis( angle * Mathf.Rad2Deg, new Vector3(0,1,0)));
        }
    }

    private void FollowGameObject()
    {
        SetState(new MovementEnemy(this, level, player.transform));
    }
}
