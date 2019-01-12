using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : Layers {

    public static bool GameIsPaused = false;

    [SerializeField]
    GameObject FirstMenu;

    [SerializeField]
    string nameFirstLevel;

    GameObject ActualGameObject;

    // Use this for initialization
    void Start()
    {
        ActualGameObject = FirstMenu;
    }

    public void Play()
    {
        Utils.StartFading(0.3f, Color.black, () => { SceneManager.LoadScene("Severin"); Constants.SetAllConstants(0); },
            () => { Constants.SetAllConstants(1); Manager.GetInstance().AddToStack(GameObject.Find(nameFirstLevel).GetComponent<Layers>()); });
    }

    public void StartTransition(GameObject target)
    {
        Utils.StartFading(0.3f, Color.black, () => { ActualGameObject.SetActive(false); target.SetActive(true); ActualGameObject = target; }, () => { });
    }

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
            indexSelection = value % allButtonsMenu.Count;
        }
    }

    public override void OnFocusGet()
    {
        parentUI.SetActive(true);
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
            allButtonsMenu[IndexSelection].Select();
        }
    }

    public override void OnFocusLost()
    {
        parentUI.SetActive(false);
        foreach (BaseInput bI in refInput)
        {
            bI.OnInputExecuted -= BI_OnInputExecuted;
        }
    }
}