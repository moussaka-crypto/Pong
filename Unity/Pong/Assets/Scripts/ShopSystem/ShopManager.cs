using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{

    public GameObject BuyOverlay;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void openBuyOverlay()
    {
        Debug.Log("");
        LeanTween.scale(BuyOverlay, Vector3.one, 1f).setEase(LeanTweenType.easeOutElastic);
    }

    public void closeBuyOverlay()
    {
         LeanTween.scale(BuyOverlay, Vector3.zero, 0.5f);
    }
}
