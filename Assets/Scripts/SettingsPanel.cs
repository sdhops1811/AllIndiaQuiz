using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{

    public GameObject music_off, sound_off;
    public Text txt_setId, txt_warningMsg;
    public GameObject resetCurSet, warningPopup;
    public GameObject homeScreen, setSelectionScreen, quizScreen, rateUsPanel;

    private int temp;   // temp = 1 -> ResetSet and temp = 2 -> ResetGame
    public Color disabledColor, activeColor;

    public AudioManager audioManager;

    public bool isMusicPressed;

    void OnEnable()
    {
        if (txt_setId.text == "0")      // 0 - Means not from Quiz Activity 
        {
            resetCurSet.GetComponent<Button>().interactable = false;
            resetCurSet.transform.Find("Text").GetComponent<Text>().color = disabledColor;

        }
        else
        {
            resetCurSet.GetComponent<Button>().interactable = true;
            resetCurSet.transform.Find("Text").GetComponent<Text>().color = activeColor;
       //     resetCurSet.transform.GetChild(0).GetComponent<Text>().color = new Color(219,219,219,255);
        }

        if (PlayerPrefs.GetInt(SingletonClass.instance.IS_MUSIC_OFF, 0) == 1)
        {
            music_off.SetActive(true);
        }
        else
        {
            music_off.SetActive(false);
        }

        if (PlayerPrefs.GetInt(SingletonClass.instance.IS_SOUND_OFF, 0) == 1)
        {
            sound_off.SetActive(true);
        }
        else
        {
            sound_off.SetActive(false);
        }
    }

    public void BtnMusic()
    {
        if (PlayerPrefs.GetInt(SingletonClass.instance.IS_MUSIC_OFF, 0) == 1)
        {
            Debug.Log("music on");
            music_off.SetActive(false);
            PlayerPrefs.SetInt(SingletonClass.instance.IS_MUSIC_OFF, 0);

            if (!isMusicPressed)
            {
                audioManager.PlayMusicFirstTime();
            }
            else
            {
                audioManager.PlayMusic();
            }
            if (SingletonClass.instance.IS_FIREBASE)
            {
                Firebase.Analytics.FirebaseAnalytics
                 .LogEvent("MusicOn");
            }
        }
        else
        {
            Debug.Log("music off");
            music_off.SetActive(true);
            PlayerPrefs.SetInt(SingletonClass.instance.IS_MUSIC_OFF, 1);
            audioManager.MuteMusic();

            if (SingletonClass.instance.IS_FIREBASE)
            {
                Firebase.Analytics.FirebaseAnalytics
                 .LogEvent("MusicOff");
            }
        }
    }

    public void BtnSound()
    {
        if (PlayerPrefs.GetInt(SingletonClass.instance.IS_SOUND_OFF, 0) == 1)
        {
            Debug.Log("sound on");
            sound_off.SetActive(false);
            PlayerPrefs.SetInt(SingletonClass.instance.IS_SOUND_OFF, 0);
            if (SingletonClass.instance.IS_FIREBASE)
            {
                Firebase.Analytics.FirebaseAnalytics
                 .LogEvent("SFXOn");
            }
        }
        else
        {
            Debug.Log("sound off");
            sound_off.SetActive(true);
            PlayerPrefs.SetInt(SingletonClass.instance.IS_SOUND_OFF, 1);
            if (SingletonClass.instance.IS_FIREBASE)
            {
                Firebase.Analytics.FirebaseAnalytics
                 .LogEvent("SFXOff");
            }
        }
    }

    public void OpenRateUs()
    {
        PlayerPrefs.SetInt(SingletonClass.instance.RATE_US_SHOWN, 1);
        //rateUsPanel.SetActive(true);
        gameObject.SetActive(false);
        Application.OpenURL("market://details?id=com.oeos.allindiaquiz");

        if (SingletonClass.instance.IS_FIREBASE)
        {
            Firebase.Analytics.FirebaseAnalytics
             .LogEvent("setting_rateus");
        }

    }

    public void BtnResetSet()
    {
        temp = 1;
        txt_warningMsg.text = "All current set progress will be lost. Are you sure you wish to proceed ? ";
        warningPopup.SetActive(true);

       
    }

    public void BtnResetGame()
    {
        temp = 2;
        txt_warningMsg.text = "All progress will be lost. Are you sure you wish to proceed ? ";
        warningPopup.SetActive(true);

        if (SingletonClass.instance.IS_FIREBASE)
        {
            Firebase.Analytics.FirebaseAnalytics
             .LogEvent("setting_reset_game");
        }
    }

    public void BtnCredits()
    {

    }

    public void WarningYes()
    {
        warningPopup.SetActive(false);
        if (temp == 1)          // Reset Set
        {
            Debug.Log("Reset set");

            PlayerPrefs.DeleteKey("Set" + txt_setId.text + SingletonClass.instance.IS_COMPLETED);
            PlayerPrefs.DeleteKey("Set" + txt_setId.text + SingletonClass.instance.CURR_QUESTION_NO);
            PlayerPrefs.DeleteKey("Set" + txt_setId.text + SingletonClass.instance.CURR_SCORE);
            PlayerPrefs.DeleteKey("Set" + txt_setId.text + SingletonClass.instance.HINT_REMAINING);
            PlayerPrefs.DeleteKey("Set" + txt_setId.text + SingletonClass.instance.AD_COUNTER);
            PlayerPrefs.DeleteKey("Set" + txt_setId.text + SingletonClass.instance.PREV_BEST);

            PlayerPrefs.DeleteKey(SingletonClass.instance.LAST_UNLOCKED_SET);

            if (SingletonClass.instance.IS_FIREBASE)
            {
                Firebase.Analytics.FirebaseAnalytics
                  .LogEvent("setting_reset_set", "set_no", SingletonClass.instance.LAST_UNLOCKED_SET);
            }

            quizScreen.GetComponent<QuizScreen>().MyEnable();
         //   quizScreen.SetActive(false);
         //    setSelectionScreen.SetActive(true);
        }
        else if (temp == 2)     // Reste Game
        {
            for (int i = 0; i < SingletonClass.instance.TOTAL_SETS; i++)
            {

                PlayerPrefs.DeleteKey("Set" + (i + 1) + SingletonClass.instance.IS_COMPLETED);
                PlayerPrefs.DeleteKey("Set" + (i + 1) + SingletonClass.instance.CURR_QUESTION_NO);
                PlayerPrefs.DeleteKey("Set" + (i + 1) + SingletonClass.instance.CURR_SCORE);
                PlayerPrefs.DeleteKey("Set" + (i + 1) + SingletonClass.instance.HINT_REMAINING);
                PlayerPrefs.DeleteKey("Set" + (i + 1) + SingletonClass.instance.AD_COUNTER);
                PlayerPrefs.DeleteKey("Set" + (i + 1) + SingletonClass.instance.PREV_BEST);

                PlayerPrefs.DeleteKey("Set" + (i + 1) + SingletonClass.instance.IS_UNLOCKED);
                PlayerPrefs.DeleteKey("Set" + (i + 1) + SingletonClass.instance.IS_AD_SEEN);
                
                PlayerPrefs.DeleteKey(SingletonClass.instance.LAST_UNLOCKED_SET);
            }
            PlayerPrefs.DeleteKey("Set1" + SingletonClass.instance.IS_UNLOCKED);
           
            if (SingletonClass.instance.IS_FIREBASE)
            {
                Firebase.Analytics.FirebaseAnalytics
                  .LogEvent("setting_reset_game");
            }

            homeScreen.SetActive(true);
            quizScreen.SetActive(false);
            setSelectionScreen.SetActive(false);


        }

        
        gameObject.SetActive(false);
    }

    public void WarningNo()
    {
        warningPopup.SetActive(false);

    }
}
