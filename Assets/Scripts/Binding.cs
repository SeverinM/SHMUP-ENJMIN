using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Classe generique permettant de modifier un UI Texte dés que la valeur observé est modifié , la methode ToString() doit etre redefinit
/// </summary>
/// <typeparam name="T"></typeparam>
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
