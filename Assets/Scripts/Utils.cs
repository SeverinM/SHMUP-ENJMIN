using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Classe statique contenant plusieurs methode utiles accessibles partout
/// </summary>
public static class Utils {

    static bool isFading = false;
    static Fade fading;

    /// <summary>
    /// Cherche un gameobject dans la scene contenant un component , s'il n'y en a pas le créer a la volée
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T FindOrDefault<T>() where T : Component
    {
        T output;
        output = GameObject.FindObjectOfType<T>();
        if (output == null)
        {
            output = new GameObject().AddComponent<T>();
        }
        return output;
    }

    public static void StartFading(float duration, Color col, Action lambdaAt1 , Action lambdaAt0)
    {
        if (isFading || duration <= 0)
        {
            Debug.Log("Une transition a deja lieu");
            return;
        }

        fading = FindOrDefault<Fade>();
        fading.Init(duration, lambdaAt1, lambdaAt0, col);
    }

    public static void EndFade()
    {
        isFading = false;
        GameObject.Destroy(fading.gameObject);
    }

    //Ne marche que si la camera regarde parfaitement vers le bas
    public static bool IsInCamera(Vector3 input, float distance)
    {
        Vector3 leftBottom = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, distance));
        Vector3 leftTop = Camera.main.ScreenToWorldPoint(new Vector3(0, Camera.main.pixelHeight, distance));
        Vector3 rightBottom = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, 0, distance));
        Vector3 rightTop = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, distance));

        float XRelative = Mathf.Abs(leftBottom.x - input.x) / Mathf.Abs(leftBottom.x - rightBottom.x);
        float ZRelative = Mathf.Abs(leftBottom.z - input.z) / Mathf.Abs(leftBottom.z - leftTop.z);

        return (XRelative <= 1 && ZRelative <= 1);
    }
}
