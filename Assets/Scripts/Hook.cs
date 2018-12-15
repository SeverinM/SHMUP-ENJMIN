using UnityEngine;
using System.Collections;

public class Hook : MonoBehaviour
{
    public GameObject hook;
    public GameObject hookHolder;

    public float hookTravelSpeed;
    public float playerTravelSpeed;

    public bool fired;
    public bool hooked;

    public float maxDistance;
    public float currentDistance;


    public void ShootHook()
    {
        hook.transform.Translate(Vector3.forward * Time.deltaTime * hookTravelSpeed);
        currentDistance = Vector3.Distance(transform.position, hook.transform.position);
    }


    public void ReturnHook()
    {

    }
}
