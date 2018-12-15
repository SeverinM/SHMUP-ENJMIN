using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementEnnemy : State
{
    Transform trsf;
    Vector3 targetPosition;
    Vector3 originPosition;

    public MovementEnnemy(Character chara, Transform trsf, bool followTransform = true) : base(chara)
    {
        if (followTransform)
        {
            this.trsf = trsf;
        }

        targetPosition = trsf.position - character.transform.position;
    }

    public MovementEnnemy(Character chara, Vector3 position) : base(chara)
    {
        targetPosition = position;
    }

    public override void EndState()
    {
    }

    public override void InterpretInput(BaseInput.TypeAction typeAct, BaseInput.Actions acts, Vector2 val)
    {
    }

    public override void NextState()
    {
        character.SetState(null);
    }

    public override void StartState()
    {
        originPosition = character.transform.position;
    }

    public override void UpdateState()
    {
        Vector3 deltaPosition = targetPosition;

        if (trsf != null)
        {
            deltaPosition = trsf.position - character.transform.position;

            if (Vector3.Distance(trsf.position,character.transform.position) <= Mathf.Abs(character.transform.position.y - trsf.position.y) + 0.01f)
            {
                character.SetState(null);
            }
        }
        else
        {
            Debug.Log(Vector3.Distance(originPosition + targetPosition, character.transform.position));
            if (Vector3.Distance(originPosition + targetPosition, character.transform.position) < 0.1f)
            {
                character.SetState(null);
            }
        }
      
        character.Move(deltaPosition);
    }
}
