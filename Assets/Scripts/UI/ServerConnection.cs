using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class ServerConnection
{
    private MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
    private string secretKey = "LaRéponseALaQuestionEst42!"; // La même clé est stockée dans le serveur

    public Text scoreDisplay;
    public string addScoreURL = "https://jiexdrop.herokuapp.com/addScore.php?"; // Ajout score
    public string highscoreURL = "https://jiexdrop.herokuapp.com/display.php";


    // On se connecte au serveur et on lui passe un nom, un score et un hash
    public IEnumerator PostScores(string name, int score)
    {
        // Le hash est composé d'un nom, d'un score et de la clé secrète
        string hash = Md5Sum(name + score + secretKey);

        string post_url = addScoreURL + "name=" + WWW.EscapeURL(name) + "&score=" + score + "&hash=" + hash;

        // On se connecte au site GET
        WWW hs_post = new WWW(post_url);
        yield return hs_post; // Attendre le résultat

        if (hs_post.error != null)
        {
            Debug.Log("Erreur envoi score: " + hs_post.error);
        }
    }

    // Obtenir les scores de la méthode display
    IEnumerator GetScores()
    {
        scoreDisplay.text = "Loading Scores";
        WWW hs_get = new WWW(highscoreURL);
        yield return hs_get;

        if (hs_get.error != null)
        {
            scoreDisplay.text = "Erreur reception score";
            Debug.Log("Erreur reception score: " + hs_get.error);
        }
        else
        {
            scoreDisplay.text = hs_get.text; // affichage des scores en jeu
        }
    }

    public string Md5Sum(string strToEncrypt)
    {
        System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
        byte[] bytes = ue.GetBytes(strToEncrypt);

        // Encrypter bytes
        byte[] hashBytes = md5.ComputeHash(bytes);

        // Retourner les bytes convertis en md5 en string
        string hashString = "";

        for (int i = 0; i < hashBytes.Length; i++)
        {
            hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
        }

        return hashString.PadLeft(32, '0');
    }
}

