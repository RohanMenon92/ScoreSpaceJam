using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public TMPro.TextMeshProUGUI highScore;
    public Toggle invertControl;
    // Start is called before the first frame update
    void Start()
    {
        string newText = "High Score: " + PlayerPrefs.GetInt(GameConstants.HighScorePlayerPref).ToString();
        highScore.text = newText;
        float invertControlPref = PlayerPrefs.GetFloat(GameConstants.invertControlPref);
        if(invertControlPref != 1.0f && invertControlPref != -1.0f)
        {
            PlayerPrefs.SetFloat(GameConstants.invertControlPref, 1.0f);
        }
        invertControl.isOn = PlayerPrefs.GetFloat(GameConstants.invertControlPref) == -1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GotToPlayScene()
    {
        SceneManager.LoadScene(1);
    }

    public void OnInvertControlToggle()
    {
        PlayerPrefs.SetFloat(GameConstants.invertControlPref, invertControl.isOn ? -1.0f : 1.0f);
    }
}
