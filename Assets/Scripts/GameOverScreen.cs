using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class GameOverScreen : MonoBehaviour
{
    public Text txt_setId;

    private int SET_ID;

    public GameObject homeScreen, setSelectionScreen, quizScreen, creditsPanel;

    public GameObject panelSettings, rateUs_panel;
    
    public void OpenSettings()
    {
        panelSettings.transform.Find("setId").GetComponent<Text>().text = "0";
        panelSettings.SetActive(true);

    }

    void OnEnable()
    {
        SET_ID = Convert.ToInt16(txt_setId.text);
        transform.Find("txt_completedSet").GetComponent<Text>().text = "You have completed Set "+SET_ID;

        transform.Find("ScorePanel").Find("txt_score").GetComponent<Text>().text = "Your Score: <b>" + PlayerPrefs.GetInt("Set" + SET_ID + SingletonClass.instance.CURR_SCORE, 0) + "</b>";
        transform.Find("ScorePanel").Find("txt_prevBest").GetComponent<Text>().text = "Previous Best: <b>" + PlayerPrefs.GetInt("Set" + SET_ID + SingletonClass.instance.PREV_BEST, 0) + "</b>";

        //int curr_score = PlayerPrefs.GetInt("Set" + SET_ID + SingletonClass.instance.CURR_SCORE, 0);

        //if (curr_score > PlayerPrefs.GetInt("Set" + (SET_ID) + SingletonClass.instance.PREV_BEST, 0))
        //{
        //    PlayerPrefs.SetInt("Set" + (SET_ID) + SingletonClass.instance.PREV_BEST, curr_score);
        //}

    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void Continue()
    {
        if (SET_ID == SingletonClass.instance.TOTAL_SETS) {
            // All the sets have completed and Go To MainScreen
            setSelectionScreen.SetActive(true);
            gameObject.SetActive(false);
        }
        else
        {
            //if (SET_ID == 1 && PlayerPrefs.GetInt(SingletonClass.instance.RATE_US_SHOWN) == 0)
            //{
            //    PlayerPrefs.SetInt(SingletonClass.instance.RATE_US_SHOWN , 1);
            //    rateUs_panel.SetActive(true);
            //}

            quizScreen.transform.Find("txt_setId").GetComponent<Text>().text = (SET_ID+1).ToString();
            quizScreen.SetActive(true);
            gameObject.SetActive(false);

            if (SingletonClass.instance.IS_FIREBASE)
            {
                Firebase.Analytics.FirebaseAnalytics
                 .LogEvent("set_completion_continue", "set_no", SET_ID);
            }
        }
    }

    public void ReplaySet()
    {
        PlayerPrefs.SetInt("Set" + SET_ID + SingletonClass.instance.CURR_QUESTION_NO, 0);
        PlayerPrefs.SetInt("Set" + SET_ID + SingletonClass.instance.CURR_SCORE, 0);
        PlayerPrefs.SetInt("Set" + SET_ID + SingletonClass.instance.IS_COMPLETED, 0);
        PlayerPrefs.SetInt("Set" + SET_ID + SingletonClass.instance.HINT_REMAINING, 5);

        quizScreen.transform.Find("txt_setId").GetComponent<Text>().text = SET_ID.ToString();

        quizScreen.SetActive(true);

        gameObject.SetActive(false);

        if (SingletonClass.instance.IS_FIREBASE)
        {
            Firebase.Analytics.FirebaseAnalytics
             .LogEvent("set_completion_replay", "set_no", SET_ID);
        }
        
    }

    public void Share()
    {
        if (SingletonClass.instance.IS_FIREBASE)
        {
            Firebase.Analytics.FirebaseAnalytics
             .LogEvent("gameover_share");
        }
        StartCoroutine(TakeSSAndShare());
    }

    IEnumerator TakeSSAndShare()
    {

        yield return new WaitForEndOfFrame();

        Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ss.Apply();

        string filePath = Path.Combine(Application.temporaryCachePath, "shared img.png");
        File.WriteAllBytes(filePath, ss.EncodeToPNG());

        // To avoid memory leaks
        Destroy(ss);

        string cur_score = PlayerPrefs.GetInt("Set" + SET_ID + SingletonClass.instance.CURR_SCORE, 0).ToString();
        new NativeShare().AddFile(filePath).SetSubject("All India Quiz").SetText("I just scored " + cur_score + " points in All India Quiz! Can you beat my score?\n PlayStore: https://play.google.com/store/apps/details?id=com.oeos.allindiaquiz \n AppStore: https://apps.apple.com/us/app/all-india-quiz/id1504230585?ls=1").Share();

        // Share on WhatsApp only, if installed (Android only)
        //if (NativeShare.TargetExists("com.facebook"))
        //    new NativeShare().AddFile(filePath).SetText("I just scored " + SingletonClass.instance.CURR_SCORE + " points in All India Quiz! Can you beat my score? https://market://details?id=com.oeos.allindiaquiz").SetTarget("com.whatsapp").Share();

    }
}
