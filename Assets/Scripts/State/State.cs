using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Classe permettant de passer des données entre chaque etat
/// </summary>
public class Context
{
    Dictionary<string, object> genericValues = new Dictionary<string, object>();

    public T ValuesOrDefault<T>(string key, T defaultValue)
    {
        if (!genericValues.ContainsKey(key))
        {
            return defaultValue;
        }
        else
        {
            T value = (T)genericValues[key];
            if (value != null)
            {
                return value;
            }
            else
            {
                return defaultValue;
            }
        }
    }

    public void SetInDictionary(string key, object value)
    {
        genericValues[key] = value;
    }
}

/// <summary>
/// Classe de base de tous les etats, il est possible de passer des parametres personnalisés entre etats via les constructeurs
/// </summary>
public abstract class State
{
    /// <summary>
    /// Reference de l'objet gerant la machine d'etat 
    /// </summary>
    protected Character character;

    /// <summary>
    /// Le constructeur doit toujours passer en reference l'objet sur lequel les etats travaillent
    /// </summary>
    /// <param name="character"></param>
    public State(Character character)
    {
        this.character = character;
    }

    /// <summary>
    /// Cette methode est appellé quand l'etat est sur le point d'etre remplacé par un autre, sans tenir compte de l'etat suivant
    /// </summary>
    public virtual void EndState() { }

    /// <summary>
    /// Cette methode est appellé a chaque frame par le personnage 
    /// </summary>
    public virtual void UpdateState() { }

    /// <summary>
    /// Cette methode est appellé lorsque l'objet rentre dans l'etat , il est appellé juste apres le constructeur
    /// </summary>
    public virtual void StartState() { }

    /// <summary>
    /// Methode appellé pour selectionner l'etat suivant , ce n'est pas obligatoire : il est possible de directement appellé SetState dans les autres methodes
    /// </summary>
    public virtual void NextState() { }

    /// <summary>
    /// Methode appellé a chaque fois qu'un input a changé , si la methode n'est jamais appellé verifier si le layer cable les evenements
    /// </summary>
    /// <param name="typeAct">Bouton appuyé ? relaché ? maintenu ?</param>
    /// <param name="acts">Quel action ? </param>
    /// <param name="val">Quel valeur ?  (utilisé uniquement dans certains cas comme la souris ou les joystick)</param>
    public virtual void InterpretInput(BaseInput.TypeAction typeAct, BaseInput.Actions acts, Vector2 val) { }
}
