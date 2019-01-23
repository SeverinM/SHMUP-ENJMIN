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

    //Raccourci
    public static void SetAllConstants(float newValue)
    {
        TimeScaleEnnemies = newValue;
        TimeScaleGenerators = newValue;
        TimeScalePlayer = newValue;
        TimeScaleBullet = newValue;
    }

    //Contexte
    public static readonly string IN_RECOVERY = "IsInRecovery";
    public static readonly string FOLLOW_AVOID = "FollowButAvoid";
    public static readonly string HOOK_MODE = "HookMode";
    public static readonly string POSITION_LAND = "PositionLand";
    public static readonly string IS_SHIELD = "IsShield";
    public static readonly string HOOK = "Hook";
    public static readonly string BARRIER = "Barrier";
    public static readonly string SPEED_WINCH = "SpeedWinch";
    public static readonly string SPEED_HOOK = "SpeedHook";
    public static readonly string RANGE_DASH = "RangeDash";
    public static readonly string COEFF_HOOK = "CoeffHook";
    public static readonly string RANGE_HOOK = "RangeHook";

    //Tags
    public static readonly string BULLET_TAG = "Bullet";
    public static readonly string SHIELD_TAG = "Shield";
    public static readonly string ENEMY_TAG = "Ennemy";
    public static readonly string WINCHABLE_TAG = "Winchable";
    public static readonly string PULLABLE_TAG = "Pullable";
}
