using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants{

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
}
