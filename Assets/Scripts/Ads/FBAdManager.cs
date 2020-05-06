using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudienceNetwork;

public class FBAdManager : MonoBehaviour
{
    private AdView adView;

    public InterstitialAd interstitialAd;
    public bool isLoaded;
    public bool didClose;

    public RewardedVideoAd rewardedVideoAd;
    public bool isRewardLoaded;
    public bool didRewardClose;

    public QuizScreen quizScreen;

    private void Awake()
    {
#if UNITY_EDITOR

#elif UNITY_ANDROID
        AudienceNetworkAds.Initialize();
#endif

    }

    private void Start()
    {
        
        if (PlayerPrefs.GetInt(SingletonClass.instance.IS_NO_ADS, 0) == 0)
        {
        //    LoadBanner();
            CreateAndLoadRewardedAd();
        }
        Debug.Log("start banner load.");

    }


    // Load Banner button
    public void LoadBanner()
    {
        if (this.adView)
        {
            this.adView.Dispose();
        }

        this.adView = new AdView("524514854938746_547354139321484", AdSize.BANNER_HEIGHT_50);
        this.adView.Register(this.gameObject);

        this.adView.AdViewDidLoad = (delegate () {
            Debug.Log("Banner loaded.");
            this.adView.Show(100);
        });
        adView.AdViewDidFailWithError = (delegate (string error) {
            Debug.Log("Banner failed to load with error: " + error);
        });
        adView.AdViewWillLogImpression = (delegate () {
            Debug.Log("Banner logged impression.");
        });
        adView.AdViewDidClick = (delegate () {
            Debug.Log("Banner clicked.");
        });

        // Initiate a request to load an ad.
        adView.LoadAd();
    }

    void OnDestroy()
    {
        // Dispose of banner ad when the scene is destroyed
        if (adView)
        {
            adView.Dispose();
        }
        Debug.Log("AdViewTest was destroyed!");
    }


    // Interstitial Ads
    public void LoadInterstitial()
    {
#if UNITY_EDITOR


#elif UNITY_ANDROID

        // Create the interstitial unit with a placement ID (generate your own on the Facebook app settings).
        // Use different ID for each ad placement in your app.
        interstitialAd = new InterstitialAd("524514854938746_552779548778943");
            
        interstitialAd.Register(gameObject);

        // Set delegates to get notified on changes or when the user interacts with the ad.
        interstitialAd.InterstitialAdDidLoad = delegate ()
        {
            Debug.Log("Interstitial ad loaded.");
            isLoaded = true;
            didClose = false;

            string isAdValid = interstitialAd.IsValid() ? "valid" : "invalid";
            Debug.Log("Ad loaded and is " + isAdValid + ".Click show to present!");
        };

        interstitialAd.InterstitialAdDidFailWithError = delegate (string error)
        {
            Debug.Log("Interstitial ad failed to load with error: " + error);
        };
        interstitialAd.InterstitialAdWillLogImpression = delegate ()
        {
            Debug.Log("Interstitial ad logged impression.");
        };
        interstitialAd.InterstitialAdDidClick = delegate ()
        {
            Debug.Log("Interstitial ad clicked.");
        };
        interstitialAd.InterstitialAdDidClose = delegate ()
        {
            didClose = true;

            Debug.Log("Interstitial ad did close.");
            if (interstitialAd != null)
            {
                interstitialAd.Dispose();
            }

            if (PlayerPrefs.GetInt(SingletonClass.instance.IS_NO_ADS, 0) == 0)
            {
                LoadInterstitial();
            }
            //// Do this to initiate ad again
            //if (PlayerPrefs.GetInt(SingletonClass.instance.IS_NO_ADS, 0) == 0)
            //{
            //    this.RequestInterstitial();
            //}
        };

        // Initiate the request to load the ad.
        interstitialAd.LoadAd();
#else

#endif
    }

    public void ShowIntestitial()
    {
        interstitialAd.Show();
    }

    public void CreateAndLoadRewardedAd()
    {
#if UNITY_EDITOR

#elif UNITY_ANDROID
         rewardedVideoAd = new RewardedVideoAd("524514854938746_547356252654606");
     
        RewardedVideoAd s2sRewardedVideoAd = new RewardedVideoAd("524514854938746_547356252654606");

        rewardedVideoAd.Register(gameObject);

        // Set delegates to get notified on changes or when the user interacts with the ad.
        rewardedVideoAd.RewardedVideoAdDidLoad = delegate ()
        {
            Debug.Log("facebook RewardedVideo ad loaded.");
           
            isRewardLoaded = true;
            didRewardClose = false;

            if (quizScreen.hint_remaining > 0)
            {
                quizScreen.btn_hint.interactable = true;
            }

            string isAdValid = rewardedVideoAd.IsValid() ? "valid" : "invalid";
        };
        rewardedVideoAd.RewardedVideoAdDidFailWithError = delegate (string error)
        {
            Debug.Log("RewardedVideo ad failed to load with error: " + error);
        };
        rewardedVideoAd.RewardedVideoAdWillLogImpression = delegate ()
        {
            Debug.Log("RewardedVideo ad logged impression.");

            Debug.Log("Give reward");
            quizScreen.GiveHintReward();
            CreateAndLoadRewardedAd();

        };
        rewardedVideoAd.RewardedVideoAdDidClick = delegate ()
        {
            Debug.Log("RewardedVideo ad clicked.");
        };

        rewardedVideoAd.RewardedVideoAdDidSucceed = delegate ()
        {
            Debug.Log("Rewarded video ad validated by server");
            Debug.Log("Give reward");
        };

        rewardedVideoAd.RewardedVideoAdDidFail = delegate ()
        {
            Debug.Log("Rewarded video ad not validated, or no response from server");
        };

        rewardedVideoAd.RewardedVideoAdDidClose = delegate ()
        {
            Debug.Log("Rewarded video ad did close.");
            didRewardClose = true;

            //Debug.Log("Give reward");
            //GiveHintReward();


            if (rewardedVideoAd != null)
            {
                rewardedVideoAd.Dispose();
            }
        };

        // Initiate the request to load the ad.
        rewardedVideoAd.LoadAd();

#else

#endif


    }

    public void ShowRewardedAd()
    {
        if (this.isRewardLoaded)
        {
            this.rewardedVideoAd.Show();
            this.isRewardLoaded = false;
        }
        else
        {
            Debug.Log("Ad not loaded. Click load to request an ad.");
        }
    }
}
