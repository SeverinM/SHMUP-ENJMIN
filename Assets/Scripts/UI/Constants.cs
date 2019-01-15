using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants{

    /// <summary>
    /// Vitesse du joueur, tout comportement du joueur doit etre coefficient de cet attribut
    /// </summary>
    static float timeScalePlayer = 1;
    public static float TimeScalePlayer
    {
        get
        {
            return timeScalePlayer;
        }

        set
        {
            timeScalePlayer = Mathf.Abs(value);
        }
    }

    /// <summary>
    /// Vitesse de l'ennemi, tout comportement des ennemis doivent etre en fonction de cet attribut
    /// </summary>
    static float timeScaleEnnemies = 1;
    public static float TimeScaleEnnemies
    {
        get
        {
            return timeScaleEnnemies;
        }

        set
        {
            timeScaleEnnemies = Mathf.Abs(value);
        }
    }

    /// <summary>
    /// TimeScale des generateurs
    /// </summary>
    static float timeScaleGenerators = 1;
    public static float TimeScaleGenerators
    {
        get
        {
            return timeScaleGenerators;
        }
        set
        {
            timeScaleGenerators = Mathf.Abs(value);
        }
    }

    /// <summary>
    /// Nom du joueur
    /// </summary>
    private static string playerName = "anonymous";
    public static string PlayerName
    {
        get
        {
            return playerName;
        }
        set
        {
            playerName = value;
        }
    }

    /// <summary>
    /// Score Total
    /// </summary>
    private static int totalScore = 0;
    public static int TotalScore
    {
        get
        {
            return totalScore;
        }
        set
        {
            totalScore = value;
        }
    }

    /// TimeScale des bullets
    /// </summary>
    static float timeScaleBullet = 1;
    public static float TimeScaleBullet
    {
        get
        {
            return timeScaleBullet;
        }
        set
        {
            timeScaleBullet = value;
        }
    }

    //Empeche le OnDestroy quand on quitte le jeu
    static bool applicationQuit;
    public static bool ApplicationQuit
    {
        get
        {
            return applicationQuit;
        }
        set
        {
            applicationQuit = value;
        }
    }

    public static System.Action Act { get; set; }
    public static System.Action Act2 { get; set; }

    //Raccourci
    public static void SetAllConstants(float newValue)
    {
        TimeScaleEnnemies = newValue;
        TimeScaleGenerators = newValue;
        TimeScalePlayer = newValue;
        TimeScaleBullet = newValue;
    }
}
