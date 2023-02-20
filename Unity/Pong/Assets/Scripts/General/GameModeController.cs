using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameModeController : MonoBehaviour
{
    public Slider diff;

    public Slider gameMode;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    /// <summary>
    /// Gets value of the slider and safes it in the PlayerPrefs under "difficulty"
    /// GameManager in PlayScene will ask for this value and customizes the settings depending on the difficulty
    /// </summary>
    public void getDifficulty(){
        Debug.Log("Difficulty: "+diff.value);
        PlayerPrefs.SetInt("difficulty",(int)diff.value);
    }
    public void getGameMode()
    {
        Debug.Log("Mode: "+gameMode.value);
        PlayerPrefs.SetInt("mode",(int)gameMode.value);
    }
}
