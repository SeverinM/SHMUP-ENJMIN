using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {

    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;

	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))

        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
	}

   public void Resume ()
    {
        Constants.SetAllConstants(1);
        pauseMenuUI.SetActive(false);
        GameIsPaused = false;
    }

    void Pause ()
    {
        Constants.SetAllConstants(0);
        pauseMenuUI.SetActive(true);
        GameIsPaused = true;
    }

    public void LoadMenu()
    {
        Utils.StartFading(1f, Color.black, () => SceneManager.LoadScene("Menu"), () => { });
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game");
        Application.Quit();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

