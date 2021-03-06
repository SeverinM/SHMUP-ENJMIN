﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class Menu : Layers
{
    protected Dictionary<GameObject, Vector3> StoredValue = new Dictionary<GameObject, Vector3>();

    [SerializeField]
    protected GameObject parentUI;

    protected List<Button> allButtonsMenu = new List<Button>();

    protected int indexSelection;
    public int IndexSelection
    {
        get
        {
            return indexSelection;
        }
        set
        {
            indexSelection = Mathf.Clamp(value, 0, allButtonsMenu.Count - 1);
        }
    }

    public override void OnFocusGet()
    {
        parentUI.SetActive(true);

        //On freeze tous les bullets
        foreach (GameObject gob in GameObject.FindGameObjectsWithTag(Constants.BULLET_TAG))
        {
            StoredValue[gob] = gob.GetComponent<Rigidbody>().velocity;
            gob.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }

        foreach(Button btn in parentUI.GetComponentsInChildren<Button>())
        {
            allButtonsMenu.Add(btn);
        }

        allButtonsMenu[0].Select();
        
        //On cable tous les inputs
        foreach (BaseInput inp in refInput)
        {
            inp.OnInputExecuted += Inp_OnInputExecuted;
        }
    }

    protected void Inp_OnInputExecuted(BaseInput.TypeAction tyAct, BaseInput.Actions acts, Vector2 values)
    {
        if (tyAct.Equals(BaseInput.TypeAction.Down) && acts.Equals(BaseInput.Actions.AllMovement))
        {
            double angle = Utils.AngleBetween(Vector2.left, values);
            //On va vers le bas
            if (angle > 70 && angle < 110)
            {
                IndexSelection++;
            }
            if (angle > -110 && angle < -70)
            {
                IndexSelection--;
            }
            allButtonsMenu[IndexSelection].Select();
        }

        if (tyAct.Equals(BaseInput.TypeAction.Down) && acts.Equals(BaseInput.Actions.Pause))
        {
            Resume();
        }
    }

    public override void OnFocusLost()
    {
        parentUI.SetActive(false);
        //On cable tous les inputs
        foreach (BaseInput inp in refInput)
        {
            inp.OnInputExecuted -= Inp_OnInputExecuted;
        }

        foreach (GameObject gob in StoredValue.Keys)
        {
            if (gob != null)
                gob.GetComponent<Rigidbody>().velocity = StoredValue[gob];
        }
    }

    public void Resume()
    {
        Constants.SetAllConstants(1);
        Manager.GetInstance().PopToStack();
    }

    public void GoToMenu()
    {
        Constants.ApplicationQuit = true;
        Constants.SetAllConstants(0);
        Utils.StartFading(1f, Color.black, () => SceneManager.LoadScene("Menu"), () => { Constants.SetAllConstants(1); Constants.ApplicationQuit = false; });
    }

    public void UpdateNickName(string nickname)
    {
        Constants.PlayerName = nickname;
    }

    public void Restart()
    {
        Manager.GetInstance().ResetCount();
        Constants.ApplicationQuit = true;
        Manager.GetInstance().PopAll();
        Constants.SetAllConstants(0);
        Utils.StartFading(1, Color.black, () => {SceneManager.LoadScene(SceneManager.GetActiveScene().name); }, () => { Constants.SetAllConstants(1); });
    }

    public void Quit()
    {
        Application.Quit();
    }
}
