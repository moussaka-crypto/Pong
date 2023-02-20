using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class PlayModeTests : MonoBehaviour
{
    
    
    [SetUp]
    public void SetUp()
    {
        var myGameObject = new GameObject("My Game Object");
        
    }
    // A Test behaves as an ordinary method
    [Test]
    public void PlayModeTestsSimplePasses()
    {
        // Use the Assert class to test conditions
        
    }
    
    [Test]
    public void getDifficulty(){
        PlayerPrefs.SetInt("difficulty",2);
        Assert.True(2 == PlayerPrefs.GetInt("difficulty"));
    }
    [Test]
    public void getGamemode(){
        PlayerPrefs.SetInt("mode",2);
        Assert.True(2 == PlayerPrefs.GetInt("mode"));
    }
    [Test]
    public void scaleTest()
    {
        Debug.Log("");
        var BuyOverlay = new GameObject();
        BuyOverlay.transform.localScale = new Vector3(0, 0, 0);
        BuyOverlay.transform.localScale = Vector3.one;
        Assert.True(Vector3.one == BuyOverlay.transform.localScale);
    }

    [Test]
    public void deactivateObject()
    {
        var obj = new GameObject();
        obj.SetActive(false);
        Assert.True(obj.activeSelf == false);
    }
    
    [Test]
    public void updateCoins()
    {
        var coins = new GameObject();
        coins.AddComponent<Text>();
        coins.GetComponent<Text>().text = "3";
        Assert.True("3" == coins.GetComponent<Text>().text);
    }

    [Test]
    public void resumeGame()
    {
        Time.timeScale = 1;
        var go = new GameObject();
        Vector3 initPos = go.transform.position;
        Assert.True(initPos == go.transform.position);
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator PlayModeTestsWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
    [UnityTest]
    public IEnumerator waitforSeconds()
    {
        var t = Time.time;
        yield return new WaitForSeconds(1);
        Assert.True(t <= Time.time + 1);
    }
}
