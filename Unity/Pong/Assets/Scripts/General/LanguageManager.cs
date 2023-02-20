using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace general
{
    public class LanguageManager : MonoBehaviour
    {
       public GameObject languages;
       private bool active = false;
       private void Start()
       {
          ChangeLocale(PlayerPrefs.GetInt("lang",0));
          languages.GetComponent<TMP_Dropdown>().value = PlayerPrefs.GetInt("langImg", 0);
          languages.GetComponent<TMP_Dropdown>().onValueChanged.AddListener(delegate
          {
             int val = 0;
             switch (languages.GetComponent<TMP_Dropdown>().value)
             {
                case 0:
                   val = 0;
                   PlayerPrefs.SetInt("langImg", 0);
                   break;
                case 1:
                   val = 1;
                   PlayerPrefs.SetInt("langImg", 1);
                   break;
                default:
                   val = 0;
                   PlayerPrefs.SetInt("langImg", 0);
                   break;

             }
             PlayerPrefs.SetInt("lang",val);
             
             ChangeLocale(val);
          });
       }
       public void ChangeLocale(int localeID)
       {
          StartCoroutine(SetLocale(localeID));
       }
       IEnumerator SetLocale(int _localeID)
       {
          active = true;
          yield return LocalizationSettings.InitializationOperation;
          LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localeID];
          active = false;
       }
    }
}