using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using SimpleJSON;

public class LeaderBoardMenu : MonoBehaviour {

    [SerializeField]
    TextMeshProUGUI names;

    [SerializeField]
    TextMeshProUGUI scores;

    [SerializeField]
    private int maxScores = 10;

    [SerializeField]
    string highscoreURL = "https://jiexdrop.herokuapp.com/display.php";

    void Start()
    {
        StartCoroutine(GetScores());
    }

    // Obtenir les scores de la méthode display
    IEnumerator GetScores()
    {
        names.text = "Loading Scores";
        WWW hs_get = new WWW(highscoreURL);
        yield return hs_get;

        if (hs_get.error != null)
        {
            names.text = "Erreur reception score";
            print("Erreur reception score: " + hs_get.error);
        }
        else
        {
            // affichage des scores en jeu
            string lines = hs_get.text;
            Debug.Log(lines);
            JSONNode node = JSON.Parse(lines);
            names.text = "";
            scores.text = "";

            int i = 0;
            foreach (JSONNode line in node) {
                names.text += line["name"] + "\n";
                scores.text += line["score"] + "\n";
                i++;
            }
        }
    }


}
