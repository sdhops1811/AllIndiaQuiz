using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
//using GoogleMobileAds.Api;
using AudienceNetwork;

public class QuizScreen : MonoBehaviour
{
    public DatabaseHandler databaseHandler;
    public List<QuestionData> questionList;

    private int setId, currQuestionNo, currScore;
    private bool is_answered;

    public Text txt_setNo, txt_questionNo, txt_score;
    public Text txt_question, txt_option1, txt_option2, txt_option3, txt_option4;

    public Color32 color_correct, color_wrong, color_defaultText;
    public GameObject [] btn_options;

    public GameObject setSelectionScreen, gameOverScreen, rateUs_panel;

    private float correctAnsDelay = 1.25f;
    private float wrongAnsDelay = 1.75f;

    //private float correctAnsDelay = .1f;
    //private float wrongAnsDelay = .1f;

    private bool is_hintUsed;
    public Button btn_hint;

    public GameObject panelSettings;

    private int adCounter;

    public int hint_remaining;
    public Text txt_hint;

    public AudioSource audioSource;
    public AudioClip aud_correctAns, aud_wrongAns, aud_hint;


    public AdMobManager adMobManager;
    public FBAdManager fBAdManager;
    public UnityAdsManager unityAdsManager;

    public bool USE_FB_ADS;

    public void OpenSettings()
    {
        panelSettings.transform.Find("setId").GetComponent<Text>().text = setId.ToString();
        panelSettings.SetActive(true);

        if (SingletonClass.instance.IS_FIREBASE)
        {
            Firebase.Analytics.FirebaseAnalytics
          .LogEvent("gamescreen_setting");
        }
    }

    private void OnEnable()
    {
        USE_FB_ADS = true;
        MyEnable();
    }

    public void MyEnable()
    {
        setId = Convert.ToInt32(transform.Find("txt_setId").GetComponent<Text>().text);

        adCounter = PlayerPrefs.GetInt("Set" + setId + SingletonClass.instance.AD_COUNTER, 0);

        currQuestionNo = PlayerPrefs.GetInt("Set" + setId + SingletonClass.instance.CURR_QUESTION_NO, 0);
        currScore = PlayerPrefs.GetInt("Set" + setId + SingletonClass.instance.CURR_SCORE, 0);

        hint_remaining = PlayerPrefs.GetInt("Set" + setId + SingletonClass.instance.HINT_REMAINING, 5);
        txt_hint.text = "" + hint_remaining + "/5";

        databaseHandler = new DatabaseHandler();
        questionList = databaseHandler.GetQuestionList(setId);

        txt_setNo.text = "Set " + setId;
        ResetData();
        ShowData();

        if (PlayerPrefs.GetInt(SingletonClass.instance.IS_NO_ADS, 0) == 0)
        {
            if (USE_FB_ADS)
            {
                fBAdManager.LoadInterstitial();
            }
            unityAdsManager.LoadInterstitial();
        }
    }
    private void OnDisable()
    {

    }

    public void SelectOption(Text txt_id)
    {
        if (!is_answered)
        {
            is_answered = true;
            btn_hint.interactable = false;

            adCounter++;
            PlayerPrefs.SetInt("Set" + setId + SingletonClass.instance.AD_COUNTER, adCounter);

            int answer = questionList[currQuestionNo - 1].answer;
            if (txt_id.text == answer.ToString())
            {
                // Correct Answer
                if (PlayerPrefs.GetInt(SingletonClass.instance.IS_SOUND_OFF, 0) == 0)
                {
                    audioSource.clip = aud_correctAns;
                    audioSource.Play();
                }
                

                txt_id.transform.parent.GetComponent<Image>().color = color_correct;
                txt_id.transform.parent.Find("Text").GetComponent<Text>().color = Color.white;
                currScore += 10;
                PlayerPrefs.SetInt("Set" + setId + SingletonClass.instance.CURR_SCORE, currScore);

                if (currQuestionNo == SingletonClass.instance.TOTAL_QUESTIONS_INSET)    // Set is Over
                {
                    PlayerPrefs.SetInt("Set" + (setId) + SingletonClass.instance.IS_COMPLETED, 1);
                    PlayerPrefs.SetInt("Set" + (setId + 1) + SingletonClass.instance.IS_UNLOCKED, 1);

                    Invoke("GoToGameOver", 1);
                }
                else
                {
                    Invoke("ShowData", correctAnsDelay);
                    Invoke("ResetData", correctAnsDelay);
                }
            }
            else
            {
                // Wrong Answer
                if (PlayerPrefs.GetInt(SingletonClass.instance.IS_SOUND_OFF, 0) == 0)
                {
                    audioSource.clip = aud_wrongAns;
                    audioSource.Play();
                }

                txt_id.transform.parent.GetComponent<Image>().color = color_wrong;
                txt_id.transform.parent.Find("Text").GetComponent<Text>().color = Color.white;

                btn_options[answer - 1].GetComponent<Image>().color = color_correct;
                btn_options[answer - 1].transform.Find("Text").GetComponent<Text>().color = Color.white;
                
                if (currQuestionNo == SingletonClass.instance.TOTAL_QUESTIONS_INSET)    // Set is Over
                {
                    PlayerPrefs.SetInt("Set" + (setId) + SingletonClass.instance.IS_COMPLETED, 1);
                    PlayerPrefs.SetInt("Set" + (setId + 1) + SingletonClass.instance.IS_UNLOCKED, 1);   // Unlock the next set

                    if (setId <= 20)
                    {
                        PlayerPrefs.SetInt(SingletonClass.instance.LAST_UNLOCKED_SET, (setId + 1));
                    }
                    Invoke("GoToGameOver", 1);
                }
                else
                {
                    Invoke("ShowData", wrongAnsDelay);
                    Invoke("ResetData", wrongAnsDelay);
                }
            }

            PlayerPrefs.SetInt("Set" + setId + SingletonClass.instance.CURR_QUESTION_NO, currQuestionNo);
        }
    }

    void GoToGameOver()
    {
        gameOverScreen.transform.Find("txt_setId").GetComponent<Text>().text = setId.ToString();
        gameOverScreen.SetActive(true);
        gameObject.SetActive(false);
    }

    public void ShowData()
    {
        if (PlayerPrefs.GetInt(SingletonClass.instance.IS_NO_ADS, 0) == 0)
        {
            if (!fBAdManager.isRewardLoaded && USE_FB_ADS)
            {
                fBAdManager.CreateAndLoadRewardedAd();
            }
            if (!unityAdsManager.isRewardAdReady)
            {
                unityAdsManager.LoadRewardedVideo();
            }
        }
        
        currQuestionNo++;

        txt_questionNo.text = "Question " + currQuestionNo;
        txt_score.text = "Score: <b>" + currScore+"</b>";

        txt_question.text = questionList[currQuestionNo - 1].question;
        txt_option1.text = questionList[currQuestionNo - 1].option1;
        txt_option2.text = questionList[currQuestionNo - 1].option2;
        txt_option3.text = questionList[currQuestionNo - 1].option3;
        txt_option4.text = questionList[currQuestionNo - 1].option4;

        is_answered = false;

        if (adCounter >= 7)
        {
            adCounter = 0;
            // Show Interstitial
            PlayerPrefs.SetInt("Set" + setId + SingletonClass.instance.AD_COUNTER, adCounter);
            //if (PlayerPrefs.GetInt(SingletonClass.instance.IS_NO_ADS, 0) == 0 && this.interstitial.IsLoaded())
            //{
               // adMobManager.ShowInterstitialAd();
            //}
            if (PlayerPrefs.GetInt(SingletonClass.instance.IS_NO_ADS, 0) == 0)
            {
                if (fBAdManager.isLoaded && USE_FB_ADS)
                {
                    fBAdManager.isLoaded = false;
                    fBAdManager.ShowIntestitial();
                }
                else
                {
                    unityAdsManager.ShowInterstitial();
                }
            }
        }

        if (setId == 2 && currQuestionNo == 5 && PlayerPrefs.GetInt(SingletonClass.instance.RATE_US_SHOWN) == 0)
        {
            PlayerPrefs.SetInt(SingletonClass.instance.RATE_US_SHOWN, 1);
            rateUs_panel.SetActive(true);
        }
    }

    void ResetData()
    {
        for (int i = 0; i < 4; i++)
        {
            btn_options[i].transform.GetChild(0).GetComponent<Text>().color = color_defaultText;
            btn_options[i].GetComponent<Image>().color = Color.white;
            btn_options[i].GetComponent<Button>().interactable = true;
        }

        if (hint_remaining <= 0 || !fBAdManager.isRewardLoaded)
        {
            btn_hint.interactable = false;
        }
        else
        {
            btn_hint.interactable = true;
        }
        btn_hint.interactable = true;

    }

    public void HintButton()
    {
        if (hint_remaining > 0 && !is_answered)
        {
            if (PlayerPrefs.GetInt(SingletonClass.instance.IS_NO_ADS, 0) == 1)
            {
                is_hintUsed = false;
                btn_hint.interactable = false;

                if (SingletonClass.instance.IS_FIREBASE)
                {
                    Firebase.Analytics.Parameter[] hintParameters = {
                      new Firebase.Analytics.Parameter(
                        "set_no", setId),
                      new Firebase.Analytics.Parameter(
                        "question_no", currQuestionNo),
                    };

                    Firebase.Analytics.FirebaseAnalytics.LogEvent(
                      "hint_used",
                      hintParameters);
                }

                GiveHintReward();
            }
            else
            {
                if (fBAdManager.isRewardLoaded && USE_FB_ADS)
                {
                    fBAdManager.ShowRewardedAd();

                    is_hintUsed = false;
                    btn_hint.interactable = false;

                    if (SingletonClass.instance.IS_FIREBASE)
                    {
                        Firebase.Analytics.Parameter[] hintParameters = {
                      new Firebase.Analytics.Parameter(
                        "set_no", setId),
                      new Firebase.Analytics.Parameter(
                        "question_no", currQuestionNo),
                    };

                        Firebase.Analytics.FirebaseAnalytics.LogEvent(
                          "hint_used",
                          hintParameters);
                    }
                }
                else if (unityAdsManager.isRewardAdReady)
                {
                    unityAdsManager.isRewardAdReady = false;
                    unityAdsManager.ShowRewardedVideo();

                    is_hintUsed = false;
                    btn_hint.interactable = false;


                    if (SingletonClass.instance.IS_FIREBASE)
                    {
                        Firebase.Analytics.Parameter[] hintParameters = {
                      new Firebase.Analytics.Parameter(
                        "set_no", setId),
                      new Firebase.Analytics.Parameter(
                        "question_no", currQuestionNo),
                    };

                        Firebase.Analytics.FirebaseAnalytics.LogEvent(
                          "hint_used",
                          hintParameters);
                    }
                }
            }

            
            //   GiveHintReward();
        }
        else
        {
            Debug.Log("No hint available");
        }
        
    }
   
    public void BackButton()
    {
        setSelectionScreen.SetActive(true);
        gameObject.SetActive(false);
    }

    public void GiveHintReward()
    {
        audioSource.clip = aud_hint;
        audioSource.Play();

        int answer = questionList[currQuestionNo - 1].answer;

        int random;

        do
        {
            random = UnityEngine.Random.Range(1, 5);
        } while (random == answer);

        for (int i = 0; i < btn_options.Length; i++)
        {
            btn_options[i].GetComponent<Button>().interactable = false;
            btn_options[i].GetComponent<Image>().color = new Color32(207, 207, 207, 78);
            btn_options[i].transform.GetChild(0).GetComponent<Text>().color = new Color32(255, 255, 255, 121);
        }

        btn_options[answer - 1].GetComponent<Button>().interactable = true;
        btn_options[random - 1].GetComponent<Button>().interactable = true;
        btn_options[answer - 1].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        btn_options[random - 1].GetComponent<Image>().color = new Color32(255, 255, 255, 255);

        btn_options[answer - 1].transform.GetChild(0).GetComponent<Text>().color = color_defaultText;
        btn_options[random - 1].transform.GetChild(0).GetComponent<Text>().color = color_defaultText;

        hint_remaining--;
        txt_hint.text = "" + hint_remaining + "/5";
        PlayerPrefs.SetInt("Set" + setId + SingletonClass.instance.HINT_REMAINING, hint_remaining);
    }




}