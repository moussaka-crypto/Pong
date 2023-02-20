using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdvertisementManager : MonoBehaviour
{
    // Start is called before the first frame update
    public int waitTime = 30;
    public float waittimer;
    public TMP_Text timer;
    public bool countdown = false;
    private bool doneWatching = false;
    private GameManager _GameManager;
    private Button pause;
    void Start()
    {
        pause = GameObject.Find("btn_pause").GetComponent<Button>();
        //countdown = true;
        _GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (countdown)
        {
            waittimer -= Time.deltaTime;
            int seconds = (int)waittimer % 60;
            timer.SetText(seconds.ToString());
            if (seconds <= 0)
            {
                countdown = false;
                doneWatching = true;
                if (_GameManager.lives < 1)
                {
                    _GameManager.lives = 1;
                    //_GameManager.ball.LaunchBall();
                }
            }
        }

        if (doneWatching)
        {
            _GameManager.closeAd();
            doneWatching = false;
            pause.interactable = true;
        }
    }
}
