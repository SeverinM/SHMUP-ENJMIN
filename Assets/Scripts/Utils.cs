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
            return;
        }

        fading = FindOrDefault<Fade>();
        fading.Init(duration, lambdaAt1, lambdaAt0, col);
    }

    public static void EndFade()
    {
        isFading = false;
        if (fading != null)
            GameObject.Destroy(fading.gameObject);
    }

    //Ne marche que si la camera regarde parfaitement vers le bas
    public static bool IsInCamera(Vector3 input, float distance)
    {
        Vector3 leftBottom = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, distance));
        Vector3 leftTop = Camera.main.ScreenToWorldPoint(new Vector3(0, Camera.main.pixelHeight, distance));
        Vector3 rightBottom = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, 0, distance));
        Vector3 rightTop = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, distance));

        float XRelative = (input.x - leftBottom.x) / Mathf.Abs(leftBottom.x - rightBottom.x);
        float ZRelative = (input.z - leftBottom.z) / Mathf.Abs(leftBottom.z - leftTop.z);

        return (XRelative <= 1 && XRelative >= 0 && ZRelative >= 0 && ZRelative <= 1);
    }

    public static Vector3 GetPositionAbsolute(Vector3 input, float distance)
    {
        //Recuperation des quatres coins
        Vector3 leftBottom = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, distance));
        Vector3 leftTop = Camera.main.ScreenToWorldPoint(new Vector3(0, Camera.main.pixelHeight, distance));
        Vector3 rightBottom = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, 0, distance));
        Vector3 rightTop = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, distance));
        Vector3 xAxis = rightBottom - leftBottom;
        Vector3 yAxis = leftTop - leftBottom;
        Vector3 finalPosition = leftBottom + (xAxis * input.x) + (yAxis * input.z);

        return finalPosition;
    }

    public static double AngleBetween(Vector2 vector1, Vector2 vector2)
    {
        double sin = vector1.x * vector2.y - vector2.x * vector1.y;
        double cos = vector1.x * vector2.x + vector1.y * vector2.y;

        return Math.Atan2(sin, cos) * (180 / Math.PI);
    }

    public static float CalculateAngle(Vector3 from, Vector3 to)
    {
        return Quaternion.FromToRotation(Vector3.up, to - from).eulerAngles.y;
    }
}
