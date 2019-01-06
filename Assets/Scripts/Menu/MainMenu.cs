using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public void PlayGame ()
    {
        Utils.StartFading(1, Color.black, () => { SceneManager.LoadScene("Severin"); }, () => { });
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}