using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class SkinsManager : MonoBehaviour
{
    private int _currenCoins;
    // Start is called before the first frame update
    void Start()
    {
        _currenCoins = PlayerPrefs.GetInt("coins");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
}
