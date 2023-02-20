using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Database;
using general;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

/// <summary>
/// Class in charge of transitions between scenes
/// </summary>
public class Scenemanager : MonoBehaviour
{
    //
    public TMP_Text coins;
    public Sprite[] backgrounds;
    public GameObject bg;
    public GameObject panel;
    public GameObject fader;
    private bool active = false;

    void Start(){
        //Scene transition
        /*try
        {
            fader.SetActive(true);
            LeanTween.scale(fader, Vector3.one, 0);
            LeanTween.scale(fader, Vector3.zero, 0.5f).setOnComplete(() => fader.GameObject().SetActive(false));
        }catch(Exception e){}*/

    

        setBackground();

        var c =PlayerPrefs.GetInt("coins",0);
        coins.text = PlayerPrefs.GetInt("coins").ToString();
        PlayerPrefs.SetInt("LastChance",0);
    }

    public void setBackground()
    {
        Debug.Log(PlayerPrefs.GetInt("2"));
        if (PlayerPrefs.HasKey("2"))
        {
            try
            {
                //bg.GetComponent<SpriteRenderer>().sprite = backgrounds[PlayerPrefs.GetInt("2") - 11];
                panel.GetComponent<Image>().sprite = backgrounds[PlayerPrefs.GetInt("2") - 11];
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            //Default Skin BG because of the animated skin, index moving one up
            if(PlayerPrefs.GetInt("2") == 21)
                panel.GetComponent<Image>().sprite = backgrounds[PlayerPrefs.GetInt("2") - 16];
                //bg.GetComponent<SpriteRenderer>().sprite = backgrounds[PlayerPrefs.GetInt("2")-16];
            else if(PlayerPrefs.GetInt("2") >= 26)
                panel.GetComponent<Image>().sprite = backgrounds[PlayerPrefs.GetInt("2") - 20];    
            //bg.GetComponent<SpriteRenderer>().sprite = backgrounds[PlayerPrefs.GetInt("2")-20];
        }
    }

    public void updateCoins()
    {
        coins.text = PlayerPrefs.GetInt("coins",0).ToString();
    }
    public void OpenMainMenu()
    { 
        //Initiate.Fade("Scenes/MainScreenScene",Color.black,8.0f);
        SceneManager.LoadScene("Scenes/MainScreenScene");
    }
    
    /// <summary>
    /// Opens the Game Selection Screen
    /// </summary>
    public void OpenGameModeSelection()
    {
        //Initiate.Fade("Scenes/GameModeSelectionScene",Color.black,8.0f);
        SceneManager.LoadScene("Scenes/GameModeSelectionScene");
    }
    /// <summary>
    /// Opens the Play Screen
    /// </summary>
    public void PlayGameCasualGame()
    {
        PlayerPrefs.SetInt("LastChance",0);
        Initiate.Fade("Scenes/CasualGameScene",Color.black,8.0f);
        //SceneManager.LoadScene("Scenes/CasualGameScene");
    }
    /// <summary>
    /// Opens the Top Ten Screen
    /// </summary>
    public void OpenTopTen()
    {
        //Initiate.Fade("Scenes/HighScoreScene",Color.black,8.0f);
        SceneManager.LoadScene("Scenes/HighScoreScene");
    }
    /// <summary>
    /// Opens the Skins Screen
    /// </summary>
    public void OpenSkins()
    {
        //Initiate.Fade("Scenes/SkinsScene",Color.black,8.0f);
        SceneManager.LoadScene("Scenes/SkinsScene");
        
    }
    /// <summary>
    /// Opens the Credits Screen
    /// </summary>
    public void OpenCredits()
    {
        SceneManager.LoadScene("Scenes/CreditsScene");
    }
    /// <summary>
    /// Opens the Main Screen
    /// </summary>

    public void PlayGameInvasion()
    {
        Initiate.Fade("Scenes/InvasionGameScene",Color.black,8.0f);
        PlayerPrefs.SetInt("LastChance",0);
        //SceneManager.LoadScene("Scenes/InvasionGameScene");
    }

    public void chooseGamemode()
    {
        if (PlayerPrefs.GetInt("mode") == 0)
        {
            PlayGameCasualGame();
        }
        else
        {
            PlayGameInvasion();
        }
    }


}
