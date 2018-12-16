using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ennemy : Character {

    public enum EnemyMovementType
    {
        FOLLOW_GAME_OBJECT,
        FOLLOW_PATH
    }

    [SerializeField]
    protected int HP = 0;

    [SerializeField]
    private GameObject player;

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
        }

    }

    private void FollowPath()
    {
        Queue<Vector3> allPos = new Queue<Vector3>();
        allPos.Enqueue(new Vector3(0, 0, -5));
        allPos.Enqueue(new Vector3(-2, 0, -2));
        allPos.Enqueue(new Vector3(2, 0, -2));

        SetState(new MovementEnnemy(this, allPos, MovementEnnemy.MovementState.START));
    }

    private void FollowGameObject()
    {
        SetState(new MovementEnnemy(this, player.transform, MovementEnnemy.MovementState.START));
    }
}
