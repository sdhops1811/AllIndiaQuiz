using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SetSelectionHandler : MonoBehaviour
{
    public GameObject pf_setList, pf_emptySpace;
    public GameObject scrollContent;
    public GameObject homeScreen, quizScreen, panelSettings;

    public ScrollRect scrollRect;
    public RectTransform contentPanel;

    private int SET_ID;

    public Color color_lockedSet;

    private RectTransform autoScrollTarget;


    void Start()
    {
        PlayerPrefs.SetInt("Set1" + SingletonClass.instance.IS_UNLOCKED, 1);
        PlayerPrefs.SetInt("Set1" + SingletonClass.instance.IS_AD_SEEN, 1);

        Invoke("Delay", 5);
    }

    void Delay()
    {
    }

    void OnEnable()
    {
        ShowData();

    }

    private void OnDisable()
    {

    }

    public void SnapTo(RectTransform target)
    {
        Canvas.ForceUpdateCanvases();

        contentPanel.anchoredPosition =
            (Vector2)scrollRect.transform.InverseTransformPoint(contentPanel.position)
            - (Vector2)scrollRect.transform.InverseTransformPoint(target.position);
    }

    public void PlaySet(Text txt_setId)
    {
        int set_id =  Convert.ToInt32(txt_setId.text.Substring(3));

        quizScreen.transform.Find("txt_setId").GetComponent<Text>().text = set_id.ToString();
        
        if (SingletonClass.instance.IS_FIREBASE)
        {
            Firebase.Analytics.FirebaseAnalytics
          .LogEvent("set_selection_play", "set_no", set_id);
        }

        quizScreen.SetActive(true);
        gameObject.SetActive(false);
    }

    public void WatchAdToUnlock(Text txt_setId)
    {
        SET_ID = Convert.ToInt32(txt_setId.text.Substring(3));

        if (SingletonClass.instance.IS_FIREBASE)
        {
            Firebase.Analytics.FirebaseAnalytics
          .LogEvent("set_selection_watchad", "set_no", SET_ID);
        }
        OpenSetAfterReward();

        // Chartboost.showRewardedVideo(CBLocation.Default);
    }

    public void ReplaySet(Text txt_setId)
    {
        int set_id =  Convert.ToInt32(txt_setId.text.Substring(3));

        int curr_score = PlayerPrefs.GetInt("Set" + set_id + SingletonClass.instance.CURR_SCORE, 0);
        if (curr_score > PlayerPrefs.GetInt("Set" + (set_id) + SingletonClass.instance.PREV_BEST, 0))
        {
            PlayerPrefs.SetInt("Set" + (set_id) + SingletonClass.instance.PREV_BEST, curr_score);
        }

        PlayerPrefs.SetInt("Set" + set_id + SingletonClass.instance.CURR_QUESTION_NO, 0);
        PlayerPrefs.SetInt("Set" + set_id + SingletonClass.instance.CURR_SCORE, 0);
        PlayerPrefs.SetInt("Set" + set_id + SingletonClass.instance.IS_COMPLETED, 0);
        PlayerPrefs.SetInt("Set" + set_id + SingletonClass.instance.HINT_REMAINING, 5);

        quizScreen.transform.Find("txt_setId").GetComponent<Text>().text = set_id.ToString();
        if (SingletonClass.instance.IS_FIREBASE)
        {
            Firebase.Analytics.FirebaseAnalytics
          .LogEvent("set_selection_replay", "set_no", set_id);
        }

        quizScreen.SetActive(true);
        gameObject.SetActive(false);

        
    }

    void ShowData()
    {
        int currQuestNo = 0;
        int currScore = 0;
        int prevBest = 0;

        //Delete Old Data
        for (int i = 0; i < scrollContent.transform.childCount; i++)
        {
            Destroy(scrollContent.transform.GetChild(i).gameObject);
        }

        for (int j = 1; j <= SingletonClass.instance.TOTAL_SETS; j++)
        {
            GameObject obj = Instantiate(pf_setList, scrollContent.transform);
            obj.transform.Find("txt_set").GetComponent<Text>().text = "Set " + j;

            if (j == 1 || PlayerPrefs.GetInt("Set" + j + SingletonClass.instance.IS_UNLOCKED, 0) == 1)
            {

                currQuestNo = PlayerPrefs.GetInt("Set" + j + SingletonClass.instance.CURR_QUESTION_NO, 0);
                currScore = PlayerPrefs.GetInt("Set" + j + SingletonClass.instance.CURR_SCORE, 0);
                prevBest = PlayerPrefs.GetInt("Set" + j + SingletonClass.instance.PREV_BEST, 0);

                if (PlayerPrefs.GetInt("Set" + j + SingletonClass.instance.IS_COMPLETED, 0) == 1)       // If Set is Completed
                {
                    obj.transform.Find("txt_progress").GetComponent<Text>().text = "Progress: <b> <color=#000000ff>" + currQuestNo + "/" + SingletonClass.instance.TOTAL_QUESTIONS_INSET + "</color></b>";
                    obj.transform.Find("txt_score").GetComponent<Text>().text = "Score: <b><color=#000000ff>" + currScore + "</color></b>";
                    obj.transform.Find("txt_prevBest").GetComponent<Text>().text = "Prev Best: <b><color=#000000ff>" + prevBest + "</color></b>";

                    obj.transform.GetChild(5).gameObject.SetActive(false);  //btnPlay
                    obj.transform.GetChild(6).gameObject.SetActive(true);   // btnReplay
                }
                //else if (PlayerPrefs.GetInt("Set" + j + SingletonClass.instance.IS_AD_SEEN, 0) == 0 && j != 1)      // Watch Ad Unlock Set
                //{
                //    obj.transform.Find("txt_locked").GetComponent<Text>().text = "Watch an Ad to unlock this set";

                //    obj.transform.Find("txt_progress").GetComponent<Text>().text = "";
                //    obj.transform.Find("txt_score").GetComponent<Text>().text = "";
                //    obj.transform.Find("txt_prevBest").GetComponent<Text>().text = "";

                //    obj.transform.GetChild(5).gameObject.SetActive(false);  //btnPlay
                //    obj.transform.GetChild(7).gameObject.SetActive(true);   // btnWatchAd
                //}
                else                               
                {
                    // Set is unlocked open Quiz Screen
                    obj.transform.Find("txt_progress").GetComponent<Text>().text = "Progress: <b> <color=#000000ff>" + currQuestNo + "/" + SingletonClass.instance.TOTAL_QUESTIONS_INSET + "</color></b>";
                    obj.transform.Find("txt_score").GetComponent<Text>().text = "Score: <b><color=#000000ff>" + currScore + "</color></b>";
                    obj.transform.Find("txt_prevBest").GetComponent<Text>().text = "Prev Best: <b><color=#000000ff>" + prevBest + "</color></b>";
                    autoScrollTarget = obj.GetComponent<RectTransform>();
                }
            }
            else
            {
                obj.transform.Find("txt_locked").GetComponent<Text>().text = "Complete Set " + (j - 1) + " to unlock this set";
                obj.transform.Find("txt_locked").gameObject.SetActive(true);
                obj.transform.Find("txt_progress").gameObject.SetActive(false);
                obj.transform.Find("txt_score").gameObject.SetActive(false);
                obj.transform.Find("txt_prevBest").gameObject.SetActive(false);

                obj.transform.GetChild(5).gameObject.SetActive(false);  // btnPlay
                obj.transform.GetChild(8).gameObject.SetActive(true);   // BtnLocked

                obj.GetComponent<Image>().color = color_lockedSet;
            }
        }

        Instantiate(pf_emptySpace, scrollContent.transform);


        //   SnapTo(autoScrollTarget);
    }

    //void didCompleteRewardedVideo(CBLocation location, int reward)
    //{
    //    Debug.Log("reward video completed");
    //    OpenSetAfterReward();
    //}

    void OpenSetAfterReward()
    {
        PlayerPrefs.SetInt("Set" + SET_ID + SingletonClass.instance.IS_AD_SEEN, 1);
        PlayerPrefs.SetInt("Set" + SET_ID + SingletonClass.instance.IS_UNLOCKED, 1);

        ShowData();
    }

    public void OpenSettings()
    {
        panelSettings.transform.Find("setId").GetComponent<Text>().text = "0";
        panelSettings.SetActive(true);

        if (SingletonClass.instance.IS_FIREBASE)
        {
            Firebase.Analytics.FirebaseAnalytics
             .LogEvent("set_selection_settings");
        }
    }

    public void BackButton()
    {
        homeScreen.SetActive(true);
        gameObject.SetActive(false);
    }
}
