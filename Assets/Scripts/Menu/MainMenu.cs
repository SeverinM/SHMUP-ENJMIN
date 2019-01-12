using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    [SerializeField]
    [Tooltip("Nom de la prochaine scène à charger")]
    string sceneToLoad;

    public void PlayGame ()
    {
        Utils.StartFading(1, Color.black, () => { Constants.SetAllConstants(0); SceneManager.LoadScene(sceneToLoad); }, () => { Constants.SetAllConstants(1); });
    }

    public void SetPlayerName(string playerName)
    {
        Constants.PlayerName = playerName;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}