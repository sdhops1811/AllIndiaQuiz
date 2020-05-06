using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HomeScreen : MonoBehaviour
{

    public GameObject panelSettings, btn_noAds, quizScreen, setSelection;

    private void Start()
    {
    //    btn_noAds.transform.GetChild(0).GetComponent<Text>().text = "Buy at " + IAPManager.Instance.GetLocalizedPriceString(ShopProductNames.Noads);
    }

    public void OpenSettings()
    {
        panelSettings.transform.Find("setId").GetComponent<Text>().text = "0";
        panelSettings.SetActive(true);

        if (SingletonClass.instance.IS_FIREBASE)
        {
            Firebase.Analytics.FirebaseAnalytics
             .LogEvent("main_menu_setting");
        }

       
    }

    public void Play()
    {
        if (PlayerPrefs.GetInt(SingletonClass.instance.IS_FIRST_TIME, 0) == 0)
        {
            PlayerPrefs.SetInt(SingletonClass.instance.IS_FIRST_TIME, 1);
            quizScreen.transform.Find("txt_setId").GetComponent<Text>().text = "1";
            quizScreen.SetActive(true);

            if (SingletonClass.instance.IS_FIREBASE)
            {
                Firebase.Analytics.FirebaseAnalytics
                  .LogEvent("new_main_menu_play");
            }
        }
        else
        {
            quizScreen.transform.Find("txt_setId").GetComponent<Text>().text = PlayerPrefs.GetInt(SingletonClass.instance.LAST_UNLOCKED_SET, 1).ToString();
            quizScreen.SetActive(true);
        }
    }
}
