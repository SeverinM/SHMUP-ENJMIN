using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ennemy : Character {

    [SerializeField]
    protected int HP = 0;

    private void Start()
    {
        Queue<Vector3> allPos = new Queue<Vector3>();
        allPos.Enqueue(new Vector3(10, 0, 10));
        allPos.Enqueue(new Vector3(-10, 0,-10));
        SetState(new MovementEnnemy(this, allPos,true));
    }
}
