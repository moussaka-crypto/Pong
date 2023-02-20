using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Database;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

public class SkinAttributeManager : MonoBehaviour
{
    public Button but;
    public int id;
    public int type;
    public int price = 0; 
    public bool isBought = false;
    private GameObject _lock;
    private GameObject _heartShape;
    // Start is called  before the first frame update
    IEnumerator waitFor(float sec)
    {
        yield return new WaitForSeconds(sec);
    }
    void Start()
    {
        _lock = GameObject.Find("img_lock_"+id);
        _heartShape = GameObject.Find("img_check_"+id);
/*#if UNITY_EDITOR
        PlayerPrefs.SetInt("coins",100);
#endif*/
        //StartCoroutine(waitFor(2));
        UpdateSkins();
    }
    // Update is called once per frame
    void Update()
    {
        var selectedSkinID = PlayerPrefs.GetInt(type.ToString());
            if (selectedSkinID == id)
            {
                _heartShape.SetActive(true);
                _lock.SetActive(false);
                transform.Find("Text (TMP)").gameObject.SetActive(false);
                UpdateSkins();
            }
            else
            {
                if(_heartShape)
                _heartShape.SetActive(false);
            }
    }

    public void UpdateSkins(bool justBought = false)
    {

        var ds = new DataService("pong.db");
        var hs = ds.getSkinsTable();
        string[] tmpID;
        tmpID = hs.ElementAt(id-1).ToString().Split(',');
        int arrid = Int32.Parse(tmpID[0]);
        // Maybe: int price = Int32.Parse(tmpID[2]);
        int purch = Int32.Parse(tmpID[3]);
        if (purch == 1)
        {
            _lock.SetActive(false);
            isBought = true;
            transform.Find("Text (TMP)").gameObject.SetActive(false);
            //_heartShape.SetActive(false);
        }
        else
        {
            isBought = false;
            _lock.SetActive(true);
            _heartShape.SetActive(false);
        }

        if (justBought)
        {
            isBought= GameObject.Find("btn_close").GetComponent<SkinAttributeManager>().isBought;
        }
    }
}