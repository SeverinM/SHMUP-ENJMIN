using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// Classe effectuant une action dés qu'une valeur est modifé
/// </summary>
/// <typeparam name="T"></typeparam>
public class Binding<T> {
    public Action<T> ValueChanged;

    T watchedVal;
    public T WatchedValue
    {
        get
        {
            return watchedVal;
        }

        set
        {
            if (ValueChanged != null && !value.Equals(watchedVal))
            {
                ValueChanged(value);
            }
            watchedVal = value;
        }
    }

    public Binding(T initialValue, Action<T> action)
    {
        ValueChanged = action;
        WatchedValue = initialValue;
    }
}
