using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public TMPro.TextMeshProUGUI highScore;
    // Start is called before the first frame update
    void Start()
    {
        string newText = "High Score: " + PlayerPrefs.GetInt(GameConstants.HighScorePlayerPref).ToString();
        highScore.text = newText;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GotToPlayScene()
    {
        SceneManager.LoadScene(1);
    }
}
