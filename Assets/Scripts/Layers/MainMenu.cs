using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : Layers {

    [SerializeField]
    GameObject FirstMenu;
    protected GameObject parentUI;
    protected List<Button> allButtonsMenu = new List<Button>();
    GameObject ActualGameObject;

    // Use this for initialization
    void Awake()
    {
        ActualGameObject = FirstMenu;
        parentUI = FirstMenu;
        parentUI.SetActive(true);
    }

    public void Play()
    {
        Manager.GetInstance().PopToStack();
        Utils.StartFading(2f, Color.black, () => { SceneManager.LoadScene("Game"); Constants.SetAllConstants(0); },
            () => { Constants.SetAllConstants(1);});
    }

    public void StartTransition(GameObject target)
    {
        Utils.StartFading(0.3f, Color.black, () => { ActualGameObject.SetActive(false); target.SetActive(true); ActualGameObject = target;}, () => { });
    }

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
        foreach (BaseInput bI in refInput)
        {
            bI.OnInputExecuted += BI_OnInputExecuted;
        }

        foreach (Button btn in parentUI.GetComponentsInChildren<Button>())
        {
            allButtonsMenu.Add(btn);
        }

        allButtonsMenu[0].Select();
    }

    private void BI_OnInputExecuted(BaseInput.TypeAction tyAct, BaseInput.Actions acts, Vector2 values)
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
            if (allButtonsMenu != null)
                allButtonsMenu[IndexSelection].Select();
        }
    }

    public override void OnFocusLost()
    {
        foreach (BaseInput bI in refInput)
        {
            bI.Reset();
        }
        allButtonsMenu = null;
    }
}