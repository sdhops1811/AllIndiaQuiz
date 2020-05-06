using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class UnityAdsManager : MonoBehaviour, IUnityAdsListener
{
    string gameId = "3404405";
    bool testMode = true;
    string bannerId = "UnityBanner";
    string interstitialId = "InterstitialAd";
    string rewardedId = "rewardedVideo";

    public bool isRewardAdReady;

    public QuizScreen quizScreen;

    private void Awake()
    {
        Advertisement.Initialize(gameId, testMode);
        Advertisement.AddListener(this);
    }

    void Start()
    {
        Advertisement.AddListener(this);
    }

    public void ShowBannerAds()
    {
        if (PlayerPrefs.GetInt(SingletonClass.instance.IS_NO_ADS, 0) == 0)
        {
            StartCoroutine(ShowBannerWhenReady());
        }
    }

    // Banner
    IEnumerator ShowBannerWhenReady()
    {
        while (!Advertisement.IsReady(bannerId))
        {
            yield return new WaitForSeconds(0.5f);
        }
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        Advertisement.Banner.Show(bannerId);
    }

    public void LoadInterstitial()
    {
        Advertisement.Load(interstitialId);
    }

    //Interstitial
    public void ShowInterstitial()
    {
        Advertisement.Show(interstitialId);
    }

    // Rewarded Video
    public void LoadRewardedVideo()
    {
        Debug.Log("load reward ad");
        Advertisement.AddListener(this);
        Advertisement.Initialize(gameId, testMode);
    }

    public void ShowRewardedVideo()
    {
        Advertisement.Show(rewardedId);
    }

    public void OnUnityAdsReady(string placementId)
    {
        if (placementId == rewardedId)
        {
            Debug.Log("Reward ad loaded");
            isRewardAdReady = true;
        }
//        throw new System.NotImplementedException();
    }

    public void OnUnityAdsDidError(string message)
    {
  //      throw new System.NotImplementedException();
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        if (placementId == rewardedId)
        {
            Debug.Log("Reward ad did start");

        }
  //      throw new System.NotImplementedException();
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        if (placementId == rewardedId)
        {
            Debug.Log("Reward ad finished");

            quizScreen.GiveHintReward();
            isRewardAdReady = false;
            LoadRewardedVideo();
        }
 //       throw new System.NotImplementedException();
    }

    public void DestroyBanner()
    {
        if (!Advertisement.IsReady(bannerId))
        {
            Advertisement.Banner.Hide();
        }
    }
}
