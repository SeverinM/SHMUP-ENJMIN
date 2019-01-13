using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe de base gerant tous les input du jeu et qui renvoit toutes les modifications via des events
/// </summary>
public abstract class BaseInput {

    //Dictionnaire chargé de savoir quel controlleur est en train d'effectuer quel action pour eviter de pouvoir faire une meme action deux fois sur deux support differents
    public static Dictionary<Actions,BaseInput> lockController = new Dictionary<Actions,BaseInput>();

    public static BaseInput GetLockState(Actions acts)
    {
        if (!lockController.ContainsKey(acts))
        {
            lockController[acts] = null;
        }
        return lockController[acts];
    }

    public static void SetLockState(Actions acts, BaseInput value)
    {
        lockController[acts] = value;
    }

    /// <summary>
    /// Renvoit vrai uniquement si personne a verrouillé l'action ou si l'action est deja verrouillé par le meme objet
    /// </summary>
    /// <param name="acts"></param>
    /// <param name="asker"></param>
    /// <returns></returns>
    public static bool IsFree(Actions acts, BaseInput asker)
    {
        //Si le demandeur a deja le verrou ou qu'il n'y a pas de verrou
        if (GetLockState(acts) == null || GetLockState(acts) == asker)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //Bouton maintenu ? relaché ? appuyé ?
    public enum TypeAction
    {
        Down,
        Up,
        Pressed,
        Mouse
    }

    //Abstractions des input
    public enum Actions
    {
        AllMovement,
        Shoot,
        Rotate,
        RotateAbsolute,
        Dash,
        Pause
    }

    //Les classes enfants ne pouvant lever l'evenement il faut le faire depuis cette methode
    protected void RaiseEvent(TypeAction tyAct, Actions acts, Vector2 values)
    {
        if (OnInputExecuted != null)
            OnInputExecuted.Invoke(tyAct, acts, values);
    }

    //Plus personne n'ecoute
    public void Reset()
    {
        OnInputExecuted = null;
    }

    public abstract void UpdateInput();

    public delegate void InputEvent(TypeAction tyAct, Actions acts, Vector2 values);
    public event InputEvent OnInputExecuted;
}
