using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public void PlayGame ()
    {
        Utils.StartFading(1, Color.black, () => { Constants.SetAllConstants(0); SceneManager.LoadScene("Severin"); }, () => { Constants.SetAllConstants(1); });
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}