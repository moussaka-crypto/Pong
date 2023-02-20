using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SkinSetter : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject ball;
    public Sprite[] Skins;
    public GameObject[] Particles;
    public GameObject bar;
    public GameObject bg;
    public ParticleSystem ps;
    public Dictionary<int, Sprite> animated;
    //public List<SerializableList> nestedList = new List<SerializableList>();

    private Camera cam;


    void Start()
    {
        cam = Camera.main;
        if (PlayerPrefs.HasKey("0"))
        {
            ball.GetComponent<SpriteRenderer>().sprite = Skins[PlayerPrefs.GetInt("0")-1];
            //Default Skin Ball because of the animated skin, index moving one up
            if(PlayerPrefs.GetInt("0") >= 22)
            {
                ball.GetComponent<SpriteRenderer>().sprite = Skins[PlayerPrefs.GetInt("0")];
            }
        }
           
        if (PlayerPrefs.HasKey("1"))
        {
            foreach (var par in Particles)
            {
                par.SetActive(false);
            }
            try
            {
                Particles[PlayerPrefs.GetInt("1") - 5 - 1].SetActive(true);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
            //Default Skin Trail because of the animated skin, index moving one up
            if (PlayerPrefs.GetInt("1") >= 23)
            {
                foreach (var par in Particles)
                {
                    par.SetActive(false);
                }
            }
        }

        if (PlayerPrefs.HasKey("2"))
        {
            bg.GetComponent<SpriteRenderer>().sprite = Skins[PlayerPrefs.GetInt("2") - 1];
            //Default Skin BG because of the animated skin, index moving one up
            if (PlayerPrefs.GetInt("2") >= 24)
            {
                bg.GetComponent<SpriteRenderer>().sprite = Skins[PlayerPrefs.GetInt("2")];
            }
        }

        if (PlayerPrefs.HasKey("3"))
        {
            try
            {
                bar.GetComponent<SpriteRenderer>().sprite = Skins[PlayerPrefs.GetInt("3") - 1];
            }
            catch (Exception e){
                Debug.Log(e);
            }
            
            //Default Skin Bar because of the animated skin, index moving one up
            if (PlayerPrefs.GetInt("3") >= 25)
            {
                bar.GetComponent<SpriteRenderer>().sprite = Skins[PlayerPrefs.GetInt("3")];
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerPrefs.HasKey("2"))
        {
            //Animated look of Kraken
            if (PlayerPrefs.GetInt("2") ==  21)
            {
                var pos = cam.WorldToScreenPoint(ball.transform.position);
                if (pos.y < Screen.height / 2 + Screen.height * 0.1f)
                {
                    //unten
                    bg.GetComponent<SpriteRenderer>().sprite = Skins[PlayerPrefs.GetInt("2")];
                }
                else
                {
                    //oben
                    bg.GetComponent<SpriteRenderer>().sprite = Skins[PlayerPrefs.GetInt("2")-1];
                }
            }
        }
        
        
    }
}
