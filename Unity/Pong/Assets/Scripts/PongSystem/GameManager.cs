using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using Database;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

public class GameManager : MonoBehaviour
{
    private Rect _safeArea;
    public Ball ball;
    public GameObject ballGO;
    public GameObject playBar;
    private Vector3 playBarLastPos;

    public GameObject borderLeft;
    public GameObject borderRight;
    public GameObject borderTop;
    public GameObject borderBottom;

    public HighscoreManager hsManager;
    /**
     * Highscore Overlay
     */
    public TMP_Text txtScoreAbove;
    public TMP_Text txtNameAbove;
    public TMP_Text txtRankAbove;
    
    public TMP_Text txtScoreMid;
    public TMP_Text txtNameMid;
    public TMP_Text txtRankMid;
    
    public TMP_Text txtScoreBelow;
    public TMP_Text txtNameBelow;
    public TMP_Text txtRankBelow;

    public TMP_Text txtcountdown;
    
    public TMP_Text txtScore;
    public TMP_Text txtFinalScore;
    public TMP_Text txtFinalCoins;
    public TMP_Text txtMultiplier;

    public TMP_Text errorText;
    public TMP_InputField playerName; 

    public GameObject adOverlay;
    public GameObject gameOverOverlay;
    public GameObject pauseOverlay;
    public GameObject safeScoreOverlay;

    public Button endRound;
    public Button pause;

    private int multi = 1;
    
    public Image[] img_lives;
    private int _score;
    [FormerlySerializedAs("_lives")] public int lives;
    private int _coins;

    public GameObject shakeable; // set this via inspector
    private float _shake  = 0;
    private float _shakeAmount  = 5f;
    private float _decreaseFactor = 1.0f;
    private int _diff;
    private float _startspeed;
    private float _barscaler;

    private bool started = false;

    public bool useLeanTween = false;

    public Vector2 playBarPos;
    public float playBarSpeed;
    private float playBarLastMoved;

    public ParticleSystem explosion;


    private InvasionManager _InvasionManager;
    private GameObject[] allBlocks;

    // Start is called before the first frame update
    void Start()
    {
        ballGO.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        pause.interactable = true;
        pauseOverlay = GameObject.Find("PauseOverlay");
        if(txtcountdown != null)
        txtcountdown.GameObject().SetActive(false);
        try
        {
            _InvasionManager = GameObject.Find("InvasionManager").GetComponent<InvasionManager>();
        }
        catch (Exception e)
        {
            Debug.Log("No InvasionManager in Scene"); 
        }
        
        txtScore.SetText(0.ToString());
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 120;
        //WaitForStart();
        //ResumeGame();
        _diff = PlayerPrefs.GetInt("difficulty");
        getStartSpeed();
        playBar.transform.localScale = (new Vector3(playBar.transform.localScale.x * _barscaler, playBar.transform.localScale.y, playBar.transform.localScale.z));
        _safeArea = Screen.safeArea;
        _score = 0;
        lives = 3;
        //SetBorders(); // disabled because it does not work right now

        playBarLastPos = playBar.transform.localPosition;

    }

    public void StartWait(int myCase)
    {
        StartCoroutine(waitAndReturn(myCase));
    }

    private IEnumerator waitAndReturn(int myCase)
    {
        for (int i = 10; i >= 0; i--)
        {
            yield return new WaitForSeconds(1f);
            if (i < 4)
            {
                txtcountdown.GameObject().SetActive(true);
                txtcountdown.SetText(i.ToString());
            }
        }
        txtcountdown.SetText("");
        txtcountdown.GameObject().SetActive(false);
        switch (myCase)
        {
                case 1:
                ball.addSpeed(2);
                break;
                case 3:
                    Debug.Log("Case 3");
                var bar = GameObject.Find("PlayBar").GameObject();
                bar.transform.localScale = new Vector3(bar.transform.localScale.x/2, bar.transform.localScale.y, bar.transform.localScale.z);
                break;
        }
    }

    /// <summary>
    /// Checks which startSpeed to use, depending on the selected difficulty
    /// </summary>
    void getStartSpeed()
    {
        switch (_diff)
        {
            case 0:
                _barscaler = 1.5f;
                _startspeed = 3f;
                ball.maxSpeed = 3f;
                break;
            case 1:
                _barscaler = 1f;
                _startspeed = 5f;
                ball.maxSpeed = 10f;
                break;
            case 2:
                _barscaler = 0.75f;
                _startspeed = 8f;
                ball.maxSpeed = 20f;
                break;
        }
    }
    
    
    /// <summary>
    /// Resizes all 4 Collision Borders to fit the Safe Area
    /// </summary>
    void SetBorders()
    {
        const float thickness = 10.0f;

        borderLeft.transform.localScale = new Vector3(thickness, _safeArea.height + 100);
        borderRight.transform.localScale = new Vector3(thickness, _safeArea.height + 100);
        borderTop.transform.localScale = new Vector3(_safeArea.width + 100, thickness);
        borderBottom.transform.localScale = new Vector3(_safeArea.width + 100, thickness);
    }

    public TMP_Text fpsText;
    public float deltaTime;

    private int updateCycle = 0;
    // Update is called once per frame
    void Update()
    {
        ball.validateSpeed();
        ball.validateAngle();
        
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        fpsText.text = Mathf.Ceil (fps).ToString ();
        if (Input.touchCount > 0)
        {
            // Recognize first touch
            if (!started)
            {
                Touch firstTouch = Input.GetTouch(0);

                // Move the Ball if the screen has the finger moving.
                if (firstTouch.phase == TouchPhase.Moved)
                {
                    ballGO.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
                    ResumeGame();
                    ball.LaunchBall(_startspeed);
                    if (_InvasionManager)
                    {
                        _InvasionManager.distance = 0;
                        _InvasionManager.spawnRow();
                    }
                    
                    started = true;
                }
            }

            // PlayBar Movement
            Touch touch = Input.GetTouch(0);
            Vector3 touchDelta = new Vector3(touch.deltaPosition.x, 0.0f, 0.0f);
            
            if (touchDelta.x < 0 && playBar.transform.localPosition.x - 0.5 * playBar.transform.localScale.x > borderLeft.transform.localPosition.x)
                playBar.transform.localPosition += touchDelta;
            else if (touchDelta.x > 0 && playBar.transform.localPosition.x + 0.5 * playBar.transform.localScale.x < borderRight.transform.localPosition.x)
                playBar.transform.localPosition += touchDelta;
            
        }

        if (updateCycle >= 5)
        {
            updateCycle = 0;
            var localPosition = playBar.transform.localPosition;
            playBarSpeed = localPosition.x - playBarLastPos.x;
            playBarLastPos = localPosition;
        }
        else
        {
            updateCycle++;
        }
    }

    /// <summary>
    /// Is called by Ball, when entering a collision trigger
    /// </summary>
    public void BallTrigger(Collision2D collision2D)
    {
        Collider2D col = collision2D.collider;
        
        if (col == playBar.GetComponent<Collider2D>())
        {
            //shake = .5f;
            ball.addSpeed(0.05f);
            ball.addSideForce(playBarSpeed);
            Debug.Log("Speed: " + ballGO.GetComponent<Rigidbody2D>().velocity);
            _score = int.Parse(txtScore.GetParsedText()) + 1;
            txtScore.text = _score.ToString();
            Debug.Log(_score);
        }
        else if (col == borderBottom.GetComponent<Collider2D>())
        {
            Handheld.Vibrate();
            lives--;
            img_lives[lives].enabled = false;
            Instantiate(explosion,img_lives[lives].transform.position,img_lives[lives].transform.rotation);
            if (lives <= 0)
            {
                
                showScore();
                return;
            }

            if (_InvasionManager)
            {
                _InvasionManager.b.moveSpeed = 0;
                foreach (var block in GameObject.FindGameObjectsWithTag("Block"))
                {
                    block.GetComponent<block>().moveSpeed = 0;
                    Destroy(block);
                }
            }
           
            ball.LaunchBall(0);
            ballGO.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            started = false;
        }
    }


    public void showScore()
    { 
        allBlocks = GameObject.FindGameObjectsWithTag("Block");
        foreach (var block in allBlocks)
        {
            Destroy(block);
        }
        switch (_diff)
        {
            case 0:
                StartCoroutine(showScoreFlow(int.Parse(txtScore.GetParsedText()), txtFinalScore,txtFinalCoins,txtMultiplier,1f, 0.1f));
                //txtFinalScore.text = _score.ToString();
                //txtFinalCoins.text = (_score / 10).ToString();
                multi = 1;
                break;
            case 1:
                StartCoroutine(showScoreFlow(int.Parse(txtScore.GetParsedText()), txtFinalScore,txtFinalCoins,txtMultiplier,1f, 0.1f));
                //txtFinalScore.text = _score.ToString();
                //txtFinalCoins.text = (_score / 10).ToString();
                multi = 1;
                break; 
            case 2:
                StartCoroutine(showScoreFlow(int.Parse(txtScore.GetParsedText()), txtFinalScore,txtFinalCoins,txtMultiplier,2f, 0.1f));
                //txtFinalScore.text = _score.ToString();
                //txtFinalCoins.text = (_score / 5).ToString();
                multi = 2;
                break;
        }
        ballGO.SetActive(false);
        GameOverOverlay();
    }

    /// <summary>
    ///Animates the final Score screen and shows numbers after each other, counting them up
    /// </summary>
    /// <param name="num">Number, to count to </param>
    /// <param name="scoreText"></param>
    /// <param name="coinsText"></param>
    /// <param name="multiText"></param>
    /// <param name="multi">Difficutlty multiplier</param>
    /// <param name="sec">Animation time</param>
    /// <returns></returns>
    public IEnumerator showScoreFlow(float num, TMP_Text scoreText, TMP_Text coinsText, TMP_Text multiText ,float multi ,float sec)
    {
        endRound.interactable = false;
        float i = 0;
        //Increment Points
        while (i < num)
        {
            scoreText.SetText((i+1).ToString());
            i++;
            yield return new WaitForSeconds(sec/(1+0.1f*i));
        }

        i = 0;
        //Increment Multiplier
        while (i < multi)
        {
            multiText.SetText((i+0.1f).ToString("#.##") + "X");
            i += 0.1f;
            yield return new WaitForSeconds(sec/2);
        } 
        i = 0; 
        //Increment Coins
        while (i < (int)(num/10*multi))
        {
            coinsText.SetText((i+1f).ToString());
            i +=1f;
            _coins = (int)i + 1;
            yield return new WaitForSeconds(sec/(1+0.1f*i));
        }  
        endRound.interactable = true; 
    }

    /// <summary>
    /// Scales up the Game Over Overlay after the end of a round. 
    /// </summary>
    public void GameOverOverlay()
    {
        pause.interactable = false;
        if(txtcountdown)
        txtcountdown.GameObject().SetActive(false);
        if(_InvasionManager)  
        _InvasionManager.paused = true;
        //_InvasionManager.GameObject().SetActive(false);
        allBlocks = GameObject.FindGameObjectsWithTag("Block");
        foreach (var block in allBlocks)
        {
            Destroy(block);
        }

        _coins = _score / 10;

        ballGO.SetActive(false);
        //PauseGame();
        if (PlayerPrefs.GetInt("LastChance")  == 1)
        {
            GameObject.Find("btn_lastChance").GetComponent<Button>().interactable = false;
        } 
        if(useLeanTween)
        LeanTween.scale(gameOverOverlay, Vector3.one, 1f).setEase(LeanTweenType.easeOutElastic);
        else gameOverOverlay.transform.localScale = Vector3.one;
    }
    
    

    public void checkForHighscore()
    {
        if(_InvasionManager)
        _InvasionManager.paused = true; 
        Debug.Log("erspielte Coins:" + _coins);
        PlayerPrefs.SetInt("coins", PlayerPrefs.GetInt("coins") + _coins);
        //Classic mode
        Debug.Log("Mode received in GM: "+PlayerPrefs.GetInt("mode"));
        if (!_InvasionManager)
        {
            Debug.Log("Classicmode: Score: " + _score +  ", Niedrigster Score:  " + hsManager.FetchLowestHs());
            if (hsManager.FetchLowestHs() < _score || HighscoreManager.RetCountRows() < 10 && _score > 0)
            {
                Debug.Log("Neuer Top-10 Score ");
                ResumeGame();
                ballGO.SetActive(false);
                if (useLeanTween)
                {
                    LeanTween.scale(gameOverOverlay, Vector3.zero, 1f).setEase(LeanTweenType.easeOutElastic);
                    LeanTween.scale(safeScoreOverlay, Vector3.one, 1f).setEase(LeanTweenType.easeOutElastic)
                        .setOnComplete(PauseGame);
                }
                else
                {
                    gameOverOverlay.transform.localScale = Vector3.zero;
                    safeScoreOverlay.transform.localScale = Vector3.one;
                    pause.interactable = false;
                }

                int TopCount = HighscoreManager.RetTopCount(_score);
                int BottomCount = HighscoreManager.RetBottomCount(_score);

                //New score is top
                //subset above >= 1 && subset below < 1
                if (TopCount <1  && BottomCount >= 1)
                {
                    txtScoreAbove.text = _score.ToString();
                    txtNameAbove.text = "YOU";
                    txtRankAbove.text = "1.";
                
                    string[] mid = HighscoreManager.GetBottom(_score);
                    txtScoreMid.text = mid[1];
                    txtNameMid.text = mid[2].TrimEnd('0');
                    txtRankMid.text = "2.";
                
                    string[] below = HighscoreManager.GetBottom(Int32.Parse(mid[1]));
                    txtScoreBelow.text = below[1];
                    txtNameBelow.text = below[2].TrimEnd('0');;
                    txtRankBelow.text = "3.";
                }
                //first entry 
                else if (TopCount == 0 && BottomCount == 0)
                {
                    txtScoreAbove.text = _score.ToString();
                    txtNameAbove.text = "YOU";
                    txtRankAbove.text = "1.";
                }
                //while entries >= 1 && < 10
                else if (TopCount == 1 && BottomCount == 0)
                {
                    string[] mid = HighscoreManager.GetTop(_score);
                    txtScoreMid.text = _score.ToString(); 
                    txtNameMid.text = "YOU";
                    txtRankMid.text = "2.";
                
                    string[] above = HighscoreManager.GetTop(_score);
                    txtScoreAbove.text = above[1]; 
                    txtNameAbove.text = above[2].TrimEnd('0');
                    txtRankAbove.text = "1.";
                }
                //New score is bottom
                //subset above >= 1 && subset below < 1
                else if (TopCount >=  1 && BottomCount < 1)
                {
                    string[] mid = HighscoreManager.GetTop(_score);
                    txtScoreMid.text = mid[1]; 
                    txtNameMid.text = mid[2].TrimEnd('0');
                    ;
                    txtRankMid.text = (TopCount).ToString()+".";
                
                
                    string[] above = HighscoreManager.GetTop(Int32.Parse(mid[1]));
                    txtScoreAbove.text = above[1]; 
                    txtNameAbove.text = above[2].TrimEnd('0');
                    txtRankAbove.text = (TopCount - 1).ToString()+".";
                
                    txtScoreBelow.text = _score.ToString(); 
                    txtNameBelow.text = "YOU";
                    txtRankBelow.text = (TopCount + 1).ToString()+".";
                }
                else //New score is mid
                    //subset above && subset below >= 1 or empty
                {
                    string[] above = HighscoreManager.GetTop(_score);
                    txtScoreAbove.text = above[1]; 
                    txtNameAbove.text = above[2].TrimEnd('0');
                
                    txtRankAbove.text = TopCount.ToString()+".";

                    txtScoreMid.text = _score.ToString();
                    txtNameMid.text = "YOU";
                    txtRankMid.text = (TopCount +  1).ToString()+".";
                
                    string[] bottom = HighscoreManager.GetBottom(_score);
                    txtScoreBelow.text = bottom[1]; 
                    txtNameBelow.text = bottom[2].TrimEnd('0');
                    txtRankBelow.text = (TopCount +2).ToString()+".";
                }
            }
            else
            {
                SceneManager.LoadScene("Scenes/MainScreenScene");
            }
        }
        
        //invasion mode
        
        else if (_InvasionManager)
        {
            Debug.Log("Invasionmode: Score: " + _score +  ", Niedrigster Score:  " + hsManager.FetchLowestHsInvasion());
            int lowesths = hsManager.FetchLowestHsInvasion();
            int rowcount = HighscoreManager.RetCountRowsInvasion();
            if (hsManager.FetchLowestHsInvasion() < _score || HighscoreManager.RetCountRowsInvasion() < 10  && _score > 0)
            {
                Debug.Log("Neuer Top-10 Score ");
                ResumeGame();
                ballGO.SetActive(false);
                if (useLeanTween)
                {
                    LeanTween.scale(gameOverOverlay, Vector3.zero, 1f).setEase(LeanTweenType.easeOutElastic);
                    LeanTween.scale(safeScoreOverlay, Vector3.one, 1f).setEase(LeanTweenType.easeOutElastic)
                        .setOnComplete(PauseGame);
                }
                else
                {
                    gameOverOverlay.transform.localScale = Vector3.zero;
                    safeScoreOverlay.transform.localScale = Vector3.one;
                }
                int TopCount = HighscoreManager.RetTopCountInvasion(_score);
                int BottomCount = HighscoreManager.RetBottomCountInvasion(_score);

                //New score is top
                //subset above >= 1 && subset below < 1
                if (TopCount <1  && BottomCount >= 1)
                {
                    txtScoreAbove.text = _score.ToString();
                    txtNameAbove.text = "YOU";
                    txtRankAbove.text = "1.";

                    string[] mid = HighscoreManager.GetBottomInvasion(_score);
                    txtScoreMid.text = mid[1];
                    txtNameMid.text = mid[2].TrimEnd('0');
                    txtRankMid.text = "2.";

                    string[] below = HighscoreManager.GetBottomInvasion(Int32.Parse(mid[1]));
                    txtScoreBelow.text = below[1];
                    txtNameBelow.text = below[2].TrimEnd('0');;
                    txtRankBelow.text = "3.";
                }
                //first entry 
                else if (TopCount == 0 && BottomCount == 0)
                {
                    txtScoreAbove.text = _score.ToString();
                    txtNameAbove.text = "YOU";
                    txtRankAbove.text = "1.";
                }
                //while entries >= 1 && < 10
                else if (TopCount == 1 && BottomCount == 0)
                {
                    string[] mid = HighscoreManager.GetTopInvasion(_score);
                    txtScoreMid.text = _score.ToString(); 
                    txtNameMid.text = "YOU";
                    txtRankMid.text = "2.";
                    
                    string[] above = HighscoreManager.GetTopInvasion(_score);
                    txtScoreAbove.text = above[1]; 
                    txtNameAbove.text = above[2].TrimEnd('0');
                    txtRankAbove.text = "1.";
                }
                //New score is bottom
                //subset above >= 1 && subset below < 1
                else if (TopCount >=  1 && BottomCount < 1)
                {
                    string[] mid = HighscoreManager.GetTopInvasion(_score);
                    txtScoreMid.text = mid[1]; 
                    txtNameMid.text = mid[2].TrimEnd('0');
                    txtRankMid.text = (TopCount).ToString()+".";
                

                    string[] above = HighscoreManager.GetTopInvasion(Int32.Parse(mid[1]));
                    txtScoreAbove.text = above[1]; 
                    txtNameAbove.text = above[2].TrimEnd('0');
                    txtRankAbove.text = (TopCount - 1).ToString()+".";
                
                    txtScoreBelow.text = _score.ToString(); 
                    txtNameBelow.text = "YOU";
                    txtRankBelow.text = (TopCount + 1).ToString()+".";
                }
                else //New score is mid
                    //subset above && subset below >= 1 or empty
                {
                    string[] above = HighscoreManager.GetTopInvasion(_score);
                    txtScoreAbove.text = above[1]; 
                    txtNameAbove.text = above[2].TrimEnd('0');
                
                    txtRankAbove.text = TopCount.ToString()+".";

                    txtScoreMid.text = _score.ToString();
                    txtNameMid.text = "YOU";
                    txtRankMid.text = (TopCount +  1).ToString()+".";
                    string[] bottom = HighscoreManager.GetBottomInvasion(_score);
                    txtScoreBelow.text = bottom[1]; 
                    txtNameBelow.text = bottom[2].TrimEnd('0');
                    txtRankBelow.text = (TopCount +2).ToString()+".";
                }
            }
            else
            {
                SceneManager.LoadScene("Scenes/MainScreenScene");
            }
        }
    }

    void backtoMain()
    {
        SceneManager.LoadScene("Scenes/MainScreenScene");
    }

    public void safeTopTenScore()
    {
        if (playerName.text != "")
        {
            errorText.text = "Please enter a name!";   
            Debug.Log("Saving Score");
            if (!_InvasionManager)
            {
                hsManager.NewHighscore(_score,playerName.text,_diff);
            }
            //Invasion
            else if (_InvasionManager)
            {
                hsManager.NewHighscoreInvasion(_score,playerName.text,_diff);
            }
            ResumeGame(false);
            SceneManager.LoadScene("Scenes/MainScreenScene");
        }
        else
        {
            errorText.text ="Please enter a name";
            Debug.Log("Cannot save empty Score");
            
        }
    }

    /// <summary>
    /// Scales up the Pause Overlay, when pause button is pressed
    /// </summary>
    public void PauseOverlay()
    {
        oldVelocity = ballGO.GetComponent<Rigidbody2D>().velocity;
        ballGO.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        if(txtcountdown)
        txtcountdown.GameObject().SetActive(false);
        //pauseOverlay.SetActive(true);
        //Deactivate all Blocks for better vision 
        allBlocks = GameObject.FindGameObjectsWithTag("Block");
        foreach (var block in allBlocks)
        {
            block.GetComponent<block>().moveSpeed = 0;
            block.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        }
        ballGO.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        pauseOverlay.SetActive(true);
        if(useLeanTween)
        LeanTween.scale(pauseOverlay, Vector3.one, 0.5f).setEase(LeanTweenType.easeOutElastic).setOnComplete(()=> pauseOverlay.transform.localScale = Vector3.one);
        else pauseOverlay.transform.localScale = Vector3.one;
    }
    
    public void closeAd()
    {
        ballGO.SetActive(true); 
        ball.LaunchBall(0);
        ballGO.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        started = false;
        if (_InvasionManager)
        {
            //_InvasionManager.paused = false;
            _InvasionManager.b.moveSpeed = 0;
        }
        if(useLeanTween)
        LeanTween.scale(adOverlay, Vector3.zero, 1f).setEase(LeanTweenType.easeOutElastic);
        else adOverlay.transform.localScale = Vector3.zero;
        //ball.LaunchBall(_startspeed);
        //if(_InvasionManager) _InvasionManager.spawnRow();
        img_lives[lives-1].enabled = true;
        GameObject.Find("btn_pause").GetComponent<Button>().interactable = true;
        PlayerPrefs.SetInt("LastChance",1);
    }

    public void openAd()
    {
        foreach (var block in GameObject.FindGameObjectsWithTag("Block"))
        {
            Destroy(block);
        }
        pause.interactable = false;
        txtcountdown.gameObject.SetActive(true);
        AdvertisementManager ad = adOverlay.GetComponent<AdvertisementManager>();
        GameObject.Find("btn_pause").GetComponent<Button>().interactable = false;
        //ad.GetCountdown();
        Debug.Log("Open Ad");
        ResumeGame();
        ballGO.SetActive(false);
        if (useLeanTween)
        {
            LeanTween.scale(gameOverOverlay, Vector3.zero, 0.3f);
            LeanTween.scale(adOverlay, new Vector3(1.5f, 1.5f, 1.5f), 0.3f);
        }
        else
        {
            gameOverOverlay.transform.localScale = Vector3.zero;
            adOverlay.transform.localScale = Vector3.one;
        }
        ad.countdown = true;
    }
    /// <summary>
    /// Switches from Pause- to Game Over Overlay when user wants to end the round manually
    /// </summary>
    public void EndRoundFromPause()
    {
        AdvertisementManager ad = adOverlay.GetComponent<AdvertisementManager>();
        ad.countdown = false;
        showScore();
        ResumeGame();
        adOverlay.SetActive(false);
        if(txtcountdown)    
        txtcountdown.GameObject().SetActive(false);
        ballGO.SetActive(false);
        GameObject.Find("btn_lastChance").GetComponent<Button>().interactable = false;
        if (useLeanTween)
        {
            LeanTween.scale(pauseOverlay, Vector3.zero, 0.2f);
            LeanTween.scale(gameOverOverlay, Vector3.one, 1f).setEase(LeanTweenType.easeOutElastic);
        }
        else
        {
            pauseOverlay.transform.localScale = Vector3.zero;
            gameOverOverlay.transform.localScale = Vector3.one;
        }
    }

    public void ResumeFromPause()
    {
        if(useLeanTween)
        LeanTween.scale(pauseOverlay, Vector3.zero, 0.2f).setOnComplete(() => pauseOverlay.SetActive(false));
        else pauseOverlay.transform.localScale = Vector3.zero;
        //pauseOverlay.SetActive(false);
        if (_InvasionManager)
        {
        if (txtcountdown)
            txtcountdown.GameObject().SetActive(true);
    }
        //ResumeGame();
        if (started)
            StartCoroutine(countdown(3));
        else ballGO.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
    }

    private IEnumerator waitfor(float sec)
    {
        yield return new WaitForSeconds(sec);
    }

    private Vector2 oldVelocity;
    public void PauseGame ()
    {
        Time.timeScale = 0;
    }

    public void ResumeGame(Boolean showBall = true)
    {
        if(showBall)
        ballGO.SetActive(true);
        ballGO.GetComponent<Rigidbody2D>().velocity = oldVelocity;
    }

    /// <summary>
    /// Takes an textobject and displays a countdown 
    /// </summary>
    /// <param name="countdownTime">Start of countdown</param>
    /// <returns></returns>
    private IEnumerator countdown(float countdownTime)
    {
        pause.interactable = false;
        txtcountdown.gameObject.SetActive(true);
        for (int i = (int)countdownTime; i > 0; i--)
        {
            txtcountdown.SetText(i.ToString());
            yield return new WaitForSeconds(1);
        }
        foreach (var block in allBlocks)
        {
            block.GetComponent<block>().moveSpeed = block.GetComponent<block>().oldSpeed;
            block.SetActive(true);
        }
        txtcountdown.gameObject.SetActive(false);
        ballGO.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        ballGO.GetComponent<Rigidbody2D>().velocity = oldVelocity;
        pause.interactable = true;
    }
}