using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class block : MonoBehaviour
{
    public float moveSpeed = 1f;
    private int _strength;
    private int _startStrength;
    public TMP_Text strength;
    public GameObject borderBottom;
    private SpriteRenderer sr;
    public float oldSpeed;
    public GameManager gm;
    private bool isSpecial;
    public Sprite[] damageSprite;
    private GameObject go_PowerUp;
    private Ball ball;
    private int powerUp;
    public Sprite[] powerUps;
    public AudioSource AudioSource;

    public ParticleSystem explosion;
    // Start is called before the first frame update
    void Start()
    {
        AudioSource = GameObject.Find("ExplosionSound").GetComponent<AudioSource>();
        powerUp = Random.Range(1, 4);
        ball = GameObject.Find("Ball").GetComponent<Ball>();
        go_PowerUp = GameObject.Find("go_PowerUp");
        oldSpeed = moveSpeed;
        _startStrength = Random.Range(1, 8);
        _strength = _startStrength;
        strength.SetText(_strength.ToString());
        sr = GetComponent<SpriteRenderer>();
        if(_startStrength == 7)
        {
            transform.localScale *= (256/30);
            GetComponent<BoxCollider2D>().size /= (256 / 30);
            _strength = 1;
            isSpecial = true;
            strength.SetText(" ");
            switch (powerUp)
            {
                case 1:
                    sr.sprite = powerUps[0];
                    break;
                case 2:
                    sr.sprite = powerUps[1];
                    break;
                case 3:
                    sr.sprite = powerUps[2];
                    break;
            }
        }
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        
        GetComponentInChildren<Canvas>().overrideSorting = true;
        updateColor();
        if(!isSpecial) sr.sprite = damageSprite[_strength-1];


    }

    // Update is called once per frame
    void Update()
    {
        float distance = moveSpeed * Time.deltaTime;
        //sr.color = new Color(Random.Range(1,5) * _strength,Random.Range(1,5)* _strength, Random.Range(1,5) * _strength,100);
        // Move the object forward by the calculated distance
        transform.Translate(Vector3.down * distance);
    }

    void waitAndClose()
    {
        LeanTween.scale(go_PowerUp, Vector3.zero, 0.2f);
    }

    void updateColor()
    {
        switch (_strength)
        {
            case 1:
                sr.color = new Color(100,100,100);
                break;
            case 2:
                sr.color = new Color(100,100,100);
                break;
            case 3:
                sr.color = new Color(100,100,100);
                break;
            case 4:
                sr.color = new Color(100,100,100);
                break;
            case 5:
                sr.color = new Color(100,100,100);
                break;
            case 6:
                sr.color = new Color(100,100,100);
                break;
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the colliding object has the tag "Enemy"
        if (collision.gameObject.tag == "Ball")
        {
            _strength--;
            if(_strength > 0)
            sr.sprite = damageSprite[_strength-1];
            updateColor();
            strength.SetText(_strength.ToString());
            // Destroy the game object
            if (_strength <= 0)
            {
                //PowerUp
                if (isSpecial)
                {
                    
                    switch (powerUp)
                    {
                        case 1:
                            ball.addSpeed(-2);
                            gm.StartWait(powerUp);
                            break;
                        case 2:
                            var allBlocks = GameObject.FindGameObjectsWithTag("Block");
                            foreach (var block in allBlocks)
                            {
                                if (block.transform.position.y == transform.position.y)
                                {
                                    Instantiate(explosion, block.transform.position, block.transform.rotation);
                                    Destroy(block);
                                }
                            }
                            break;
                        case 3:

                            // Widen the object by 2 
                            var bar = GameObject.Find("PlayBar").GameObject(); 
                            bar.transform.localScale = new Vector3(bar.transform.localScale.x*2, bar.transform.localScale.y, bar.transform.localScale.z);
                            gm.StartWait(powerUp);
                            break;
                    }

                }
                var newScore = int.Parse(gm.txtScore.GetParsedText()) + _startStrength; 
                gm.txtScore.SetText(newScore.ToString());
                Handheld.Vibrate();
                AudioSource.Play();
                Instantiate(explosion,transform.position,transform.rotation);
                Destroy(gameObject);
                explosion.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            }
            
        }
        else if (collision.gameObject.tag == "Bottom" || collision.gameObject.tag == "Player") 
        {
            gm.showScore();
        }
    }
    
    //Power Up functions
    


}

