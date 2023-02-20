using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using general;
using UnityEngine.Localization.Settings;

public class LangSupplicant : MonoBehaviour
{
    private bool active = false;

    public int localeID;
    // Start is called before the first frame update
    void Start()
    {
        localeID = PlayerPrefs.GetInt("lang",0);
        ChangeLocale(localeID);
    }

    public void ChangeLocale(int localeID)
    {
        StartCoroutine(SetLocale(localeID));
    }
    IEnumerator SetLocale(int localeID)
    {
        active = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeID];
        active = false;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
