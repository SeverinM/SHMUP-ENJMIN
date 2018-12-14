using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Binding<T>{

    Text text;

    T watchedVal;
    public T WatchedValue
    {
        get
        {
            return watchedVal;
        }

        set
        {
            watchedVal = value;
            if (text != null)
            {
                text.text = watchedVal.ToString();
            }
            else
            {
                Debug.LogWarning("Un objet binding n'a pas de cible");
            }
        }
    }

    public Binding(Text firstText)
    {
        text = firstText;
    }
}
