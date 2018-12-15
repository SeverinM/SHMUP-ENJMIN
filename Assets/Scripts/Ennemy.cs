using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ennemy : Character {

    [SerializeField]
    protected int HP = 0;

    Stack<Vector3> allPositions = new Stack<Vector3>();

    private void Start()
    {
        allPositions.Push(new Vector3(2, 0, 1));
        allPositions.Push(new Vector3(4, 1, 3));

    }

    public Vector3 getNewPosition()
    {
        Vector3 output;
        if (allPositions.Count > 0)
        {
            output = allPositions.Pop();
        }
        else
        {
            output = Vector3.zero;
        }

        return output;
    }
}
