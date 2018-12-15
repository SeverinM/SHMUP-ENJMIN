using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour {

    [SerializeField]
    protected float moveSpeed = 6.0f;

    [SerializeField]
    protected State actualState;

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

    protected void Update()
    {
        if (actualState != null)
        {
            actualState.UpdateState();
        }
    }

    public void Move(Vector3 movement)
    {
        transform.Translate(new Vector3(movement.x, 0, movement.z).normalized * moveSpeed * Time.deltaTime);
    }
}
