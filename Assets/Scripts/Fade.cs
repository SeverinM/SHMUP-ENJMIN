using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Fade : MonoBehaviour {

    float duration;
    Action lambdaAt1;
    Action lambdaAt0;
    CanvasGroup grp;
    float alpha = 0;
    bool reverse = false;

    /// <summary>
    /// Lance une transition en fondu
    /// </summary>
    /// <param name="dur"> quel durée ?</param>
    /// <param name="lam"> que faire quand l'ecran est tout noir ? </param>
    /// <param name="lam2"> que faire quand la transition est finit ?</param>
    /// <param name="color"></param>
    public void Init(float dur, Action lam , Action lam2,  Color color)
    {
        DontDestroyOnLoad(gameObject);
        duration = dur;
        lambdaAt1 = lam;
        lambdaAt0 = lam2;

        if (GetComponent<Canvas>() == null)
        {
            gameObject.AddComponent<Canvas>();
        }

        if (GetComponent<CanvasGroup>() == null)
        {
            gameObject.AddComponent<CanvasGroup>();
        }

        grp = GetComponent<CanvasGroup>();

        if (GetComponent<Image>() == null)
        {
            gameObject.AddComponent<Image>();
        }

        GetComponent<Image>().color = color;

        transform.position = Camera.main.transform.position + Camera.main.transform.forward;
    }

    // Update is called once per frame
    void Update () {
		if (lambdaAt1 == null || lambdaAt0 == null || grp == null)
        {
            Debug.LogWarning("Une valeur n'est pas initialisé");
            return;
        }

        alpha += (Time.deltaTime / duration) * (reverse ? -1 : 1);
        alpha = Mathf.Clamp(alpha, 0, 1);
        
        if (alpha == 1)
        {
            lambdaAt1();
            reverse = true;
        }

        if (alpha == 0)
        {
            lambdaAt0();
            Utils.EndFade();
        }

        grp.alpha = alpha;
	}
}
