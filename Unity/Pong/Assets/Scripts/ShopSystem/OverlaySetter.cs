using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Database;
//using Button = UnityEngine.UIElements.Button;
using Image = UnityEngine.UI.Image;

public class OverlaySetter : MonoBehaviour
{
    // Start is called before the first frame update
    public Sprite currentSprite = null;
    public int currentPrice;
    public bool useLeanTween;
    private GameObject overlay;
    private SkinAttributeManager _skinAttributeManager;
    private Scenemanager _Scenemanager;
    //public ParticleSystem boughtCelebration; 


    void Start()
    {
        _Scenemanager = GameObject.Find("SceneManager").GetComponent<Scenemanager>();
        currentPrice= GetComponent<SkinAttributeManager>().price;
        currentSprite = GetComponent<Image>().sprite;
        Debug.Log(currentSprite);
        overlay = GameObject.Find("OverlayShop");
        _skinAttributeManager = GetComponent<SkinAttributeManager>();
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    
    IEnumerator wait(int sec)
    {
        yield return new WaitForSeconds(sec);
    }

    /// <summary>
    /// Whether opens a overlay to buy a skin or selects the pressed skin if its already purchased
    /// </summary>
    public void openOverlay()
    {
        if (!_skinAttributeManager.isBought)
        {
            Debug.Log(("Open  "));
            if(useLeanTween)
            LeanTween.scale(overlay, Vector3.one, 1f).setEase(LeanTweenType.easeOutElastic);
            else overlay.transform.localScale = Vector3.one;
            if (PlayerPrefs.GetInt("coins") < currentPrice)
            {
                GameObject.Find("txt_question").GetComponent<TMP_Text>().SetText("Not enough coins!");
                GameObject.Find("btn_buy").GetComponent<Button>().interactable = false;
            }
            else
            {
                GameObject.Find("txt_question").GetComponent<TMP_Text>().SetText("Do you want to buy this skin?");
                GameObject.Find("btn_buy").GetComponent<Button>().interactable = true;
            }
            GameObject.Find("btn_close").GetComponent<SkinAttributeManager>().id = _skinAttributeManager.id;
            overlay.transform.Find("img_skin").GetComponent<Image>().sprite = currentSprite;
            overlay.transform.Find("btn_buy").GetComponentInChildren<TMP_Text>().text = currentPrice.ToString();
            PlayerPrefs.SetInt("selectedID",_skinAttributeManager.id);
            PlayerPrefs.SetInt("selectedType",_skinAttributeManager.type);
        }
        else
        {
            Debug.Log("Skin ausgewählt!");
            PlayerPrefs.SetInt(_skinAttributeManager.type.ToString(),_skinAttributeManager.id);
        }
        _Scenemanager.setBackground();
    }
    

    /// <summary>
    /// Scales down the overlay 
    /// </summary>

    public void destroyOverlay()
    {
       Debug.Log("Destroy");
       if(useLeanTween)
       LeanTween.scale(overlay, Vector3.zero, 0.2f);
       else overlay.transform.localScale = Vector3.zero;
    }

    /// <summary>
    /// Checks, if user has enough coins to buy a skin. If not, user cant buy the skin
    /// </summary>
    public void buySkin()
    {
        
        if (int.Parse(overlay.transform.Find("btn_buy").GetComponentInChildren<TMP_Text>().text) <= PlayerPrefs.GetInt("coins"))
        {
            PlayerPrefs.SetInt("coins", PlayerPrefs.GetInt("coins") - int.Parse(overlay.transform.Find("btn_buy").GetComponentInChildren<TMP_Text>().text));
            Debug.Log("Item bought " + int.Parse(overlay.transform.Find("btn_buy").GetComponentInChildren<TMP_Text>().text) + " " + PlayerPrefs.GetInt("coins"));
            if(useLeanTween)
            LeanTween.scale(overlay, Vector3.zero, 0.5f).setOnComplete(_Scenemanager.updateCoins).setOnComplete(_Scenemanager.OpenSkins);
            else
            {
                overlay.transform.localScale = Vector3.zero;
                Debug.Log(_skinAttributeManager.type + _skinAttributeManager.id); 
                //_Scenemanager.OpenSkins();
                
            } 
            PlayerPrefs.SetInt(PlayerPrefs.GetInt("selectedType").ToString(), PlayerPrefs.GetInt("selectedID")); 
            GameObject.Find("txt_coins").GetComponent<TMP_Text>().SetText(PlayerPrefs.GetInt("coins").ToString());
            var ds = new DataService("pong.db");
            _skinAttributeManager = GameObject.Find("btn_close").GetComponent<SkinAttributeManager>();
            ds.buySkin(_skinAttributeManager.id);

            //if(boughtCelebration)   
            //Instantiate(boughtCelebration,transform.position,transform.rotation);
            _Scenemanager.setBackground();
        }  
        else
        {
            Debug.Log("Not enough coins");
        }
    }
}
