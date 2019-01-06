using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuHandler : MonoBehaviour {

    [SerializeField]
    GameObject MainMenu;

    GameObject ActualGameObject;

	// Use this for initialization
	void Start () {
        ActualGameObject = MainMenu;
	}
	
	public void StartTransition(GameObject target)
    {
        Utils.StartFading(0.3f, Color.black, () => { ActualGameObject.SetActive(false); target.SetActive(true); ActualGameObject = target; }, () => { });
    }
}
