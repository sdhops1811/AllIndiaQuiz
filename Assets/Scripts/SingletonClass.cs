using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonClass : MonoBehaviour
{
    public static SingletonClass instance;

    public int TOTAL_QUESTIONS_INSET, TOTAL_SETS;

    public string IS_UNLOCKED ,IS_AD_SEEN ,IS_COMPLETED;

    public string CURR_QUESTION_NO, CURR_SCORE, PREV_BEST, HINT_REMAINING;

    public string IS_MUSIC_OFF, IS_SOUND_OFF;

    public string IS_NO_ADS;

    public string AD_COUNTER;
    public string RATE_US_SHOWN;

    public string IS_FIRST_TIME;
    public string LAST_UNLOCKED_SET;

    public string app_id, banner_id, interstitial_id, rewardVideoId;

    public bool IS_FIREBASE;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);

            IS_UNLOCKED = "is_unlocked";
            IS_AD_SEEN = "is_ad_seen";
            IS_COMPLETED = "is_completed";

            CURR_QUESTION_NO = "curr_question_no";
            CURR_SCORE = "curr_score";
            PREV_BEST = "prev_best";
            HINT_REMAINING = "hint_remaining";

            IS_MUSIC_OFF = "is_music_off";
            IS_SOUND_OFF = "is_sound_off";

            RATE_US_SHOWN = "rate_us_shown";

            IS_FIRST_TIME = "is_first_time";

            LAST_UNLOCKED_SET = "last_unlocked_set";

            TOTAL_QUESTIONS_INSET = 25;
            TOTAL_SETS = 20;

            IS_NO_ADS = "is_no_ads";

            IS_FIREBASE = true;
            // Test ids
            //app_id = "ca-app-pub-3940256099942544~3347511713";
            //banner_id = "ca-app-pub-3940256099942544/6300978111";
            //interstitial_id = "ca-app-pub-3940256099942544/1033173712";
            //rewardVideoId = "ca-app-pub-3940256099942544/5224354917";

            // Live ids
            app_id = "ca-app-pub-8532573586876649~9592910025";
            banner_id = "ca-app-pub-8532573586876649/1794311057";
            interstitial_id = "ca-app-pub-8532573586876649/4037331015";
            rewardVideoId = "ca-app-pub-8532573586876649/6366667802";

        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }
    }
 
}
