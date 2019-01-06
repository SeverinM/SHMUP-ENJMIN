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
}
