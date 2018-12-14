using UnityEngine;
using System.Collections;

public class Player : Character
{
    private CharacterController controller;

    void Start()
    {
        controller = transform.GetComponent<CharacterController>();
    }

    void Update()
    {

    }
}
