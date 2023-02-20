using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
///Script which should be added to a reference Object.
/// Spawns a line of objects with a gap/ distance of 230 to each other
/// </summary>
public class Skins : MonoBehaviour
{
    public List<Button> buttons = new List<Button>(5);
    public List<GameObject> skins = new List<GameObject>(5);
    public GameObject parentView;
    /// <summary>
    /// Sets distance to left Screen border
    /// </summary>
    public int x = 100;
    /// <summary>
    /// Sets distance between objects
    /// </summary>
    public int relative_x = 230;
    private SkinsManager manager;
    public GameObject overlay;
     
    /// <summary> 
    /// Spawns every skin next to each other, depeneding on the list entries
    /// </summary>
    private void Start()
    {
        foreach (Button but in buttons)
        {
            Instantiate(but, new Vector2(x,transform.position.y - 200), Quaternion.identity,parentView.transform);
           // but.image.sprite = skin.GetComponent<SpriteRenderer>().sprite;
            x += relative_x;
        }
        
    }
}
